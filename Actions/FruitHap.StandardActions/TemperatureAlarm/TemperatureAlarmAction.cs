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

		ISwitch switchAbove;
		ISwitch switchBelow;

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

			switchAbove = sensorRepository.FindSensorOfTypeByName<ISwitch> (configuration.SwitchAbove);
			switchBelow = sensorRepository.FindSensorOfTypeByName<ISwitch> (configuration.SwitchBelow);
			if (switchAbove == null && switchBelow == null) 
			{
				logger.Error ("Cannot find switches");
			}

			switchAbove.TurnOff ();
			switchBelow.TurnOff ();
			eventBus.Subscribe<SensorEventData> (HandleSensorEvent, f => f.Sender.Name.Contains (configuration.TemperatureSensor) && f.Sender is ITemperatureSensor);
		}


		void UnSubscribe ()
		{			
			eventBus.Unsubscribe<SensorEventData> (HandleSensorEvent);
		}

		void HandleSensorEvent (SensorEventData data)
		{						
			SensorMessage sensorMessage = new SensorMessage ();
			var tempValue = data.OptionalData as TemperatureValue;
			if (tempValue.Temperature > configuration.ThresholdHot) 
			{
				logger.Info ("Temperature above upper limit");
				if (switchBelow.GetState () != SwitchState.Off) 
				{
					switchBelow.TurnOff ();
				}
				if (switchAbove.GetState () != SwitchState.On) {
					switchAbove.TurnOn ();
				}

				sensorMessage = new SensorMessage () 
				{
					TimeStamp = DateTime.Now,
					SensorName = data.Sender.Name,
					Data = CreateResponse(data,true),
					EventType = data.EventName
				};
			} 
			else if (tempValue.Temperature < configuration.ThresholdCold) 
			{
				logger.Info ("Temperature below lower limit");
				if (switchAbove.GetState () != SwitchState.Off) 
				{
					switchAbove.TurnOff ();
				}

				if (switchBelow.GetState () != SwitchState.On) {
					switchBelow.TurnOn ();
				}

				sensorMessage = new SensorMessage () 
				{
					TimeStamp = DateTime.Now,
					SensorName = data.Sender.Name,
					Data = CreateResponse(data,false),
					EventType = data.EventName
				};

			} 
			else 
			{
				switchAbove.TurnOff ();
				switchBelow.TurnOff ();
			}

		
			mqProvider.Publish (sensorMessage, configuration.RoutingKey);

		}




		private TemperatureAlarmResponse CreateResponse (SensorEventData data, bool isAbove)
		{
			if (isAbove) {
				return new TemperatureAlarmResponse () {
					NotificationText = configuration.NotificationTextAbove,
					Priority = NotificationPriority.High,
					OptionalData = new OptionalDataContainer (data.OptionalData)
				};
			} else
				return new TemperatureAlarmResponse () {
					NotificationText = configuration.NotificationTextBelow,
					Priority = NotificationPriority.High,
					OptionalData = new OptionalDataContainer (data.OptionalData)
				};
		}



        public void Dispose()
        {
            logger.DebugFormat("Dispose action {0}", this);
            UnSubscribe();
		}
	}




}

