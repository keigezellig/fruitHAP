using System;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Core.Action;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.MQ;
using FruitHAP.Common.Configuration;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using FruitHAP.Common.EventBus;
using FruitHap.StandardActions.Alarm.Configuration;
using FruitHap.Core.Action;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHap.StandardActions.TemperatureAlarm.Configuration;
using FruitHap.StandardActions.Alarm;


namespace FruitHap.StandardActions.TemperatureAlarm
{
	public class TemperatureAlarmAction : IAction
	{
		private readonly ISensorRepository sensorRepository;
		private readonly ILogger logger;
		private readonly IMessageQueueProvider mqProvider;
		private IConfigProvider<TemperatureAlarmActionConfiguration> configurationProvider;
		private const string CONFIG_FILENAME = "temperature_alarm_action.json";
		private TemperatureAlarmActionConfiguration configuration;
		private IEventBus eventBus;

		ITemperatureSensor tempSenor;
		ISwitch switchy;

		public TemperatureAlarmAction(ISensorRepository sensorRepository, 
								  ILogger logger, 
								  IConfigProvider<TemperatureAlarmActionConfiguration> configurationProvider, 
								  IMessageQueueProvider mqProvider,
								  IEventBus eventBus)
		{
			this.eventBus = eventBus;
			this.sensorRepository = sensorRepository;
			this.logger = logger;
			this.mqProvider = mqProvider;
			this.configurationProvider = configurationProvider;
		}


		public void Initialize()
		{
			
			logger.InfoFormat ("Initializing action {0}", this);
			logger.InfoFormat ("Loading configuration");
			configuration = configurationProvider.LoadConfigFromFile (Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), CONFIG_FILENAME));

			switchy = sensorRepository.FindSensorOfTypeByName<ISwitch> (configuration.Switch);
			if (tempSenor == null && switchy == null) 
			{
				logger.Error ("Cannot find sensors");
			}

			switchy.TurnOff ();
			eventBus.Subscribe<SensorEventData> (HandleSensorEvent, f => f.Sender.Name.Contains (configuration.TemperatureSensor));
		}


		void UnSubscribe ()
		{			
			eventBus.Unsubscribe<SensorEventData> (HandleSensorEvent);
		}

		void HandleSensorEvent (SensorEventData data)
		{			
			tempSenor = data.Sender as ITemperatureSensor;
			if (tempSenor != null) 
			{
				/*if (switchy.GetState () != SwitchState.On) {
					switchy.TurnOn ();
				} else {
					switchy.TurnOff ();
				}*/
					
				if (tempSenor.GetTemperature () > configuration.Threshold) {
					if (switchy.GetState() != SwitchState.On) {
						switchy.TurnOn ();
					}
					var sensorMessage = new SensorMessage () 
					{
						TimeStamp = DateTime.Now,
						SensorName = data.Sender.Name,
						Data = CreateResponse(data),
						EventType = data.EventName
					};

					mqProvider.Publish (sensorMessage, configuration.RoutingKey);

				} else 
				{
					if (switchy.GetState() == SwitchState.On) 
					{
						switchy.TurnOff ();
					}
				}
			}

		}


		private TemperatureAlarmResponse CreateResponse (SensorEventData data)
		{
			return new TemperatureAlarmResponse () {
				NotificationText = configuration.NotificationText,
				Priority = NotificationPriority.High,
				OptionalData = new OptionalDataContainer(data.OptionalData)
			};
		}



        public void Dispose()
        {
            logger.DebugFormat("Dispose action {0}", this);
            UnSubscribe();
		}
	}




}

