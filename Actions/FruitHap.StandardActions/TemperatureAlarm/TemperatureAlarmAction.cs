using System;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Core.Action;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.MQ;
using FruitHAP.Common.Configuration;
using System.IO;
using System.Reflection;
using FruitHAP.Common.EventBus;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHap.StandardActions.TemperatureAlarm.Configuration;
using FruitHAP.Core.Sensor.SensorValueTypes;


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
			//eventBus.Subscribe<SensorEventData> (HandleSensorEvent, f => f.Sender.Name.Contains (configuration.TemperatureSensor) && f.Sender is ITemperatureSensor);
		}


		void UnSubscribe ()
		{			
			eventBus.Unsubscribe<SensorEventData> (HandleSensorEvent);
		}

		void HandleSensorEvent (SensorEventData data)
		{						
			SensorMessage sensorMessage = new SensorMessage ();
			var tempValue = data.OptionalData.Content as TemperatureQuantity;
			if (tempValue.Value > configuration.ThresholdHot) 
			{
				logger.Info ("Temperature above upper limit");
				if (switchBelow.State.Value  != StateValue.Off ) 
				{
					switchBelow.TurnOff ();
				}
				if (switchAbove.State.Value != StateValue.Off) {
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
			else if (tempValue.Value < configuration.ThresholdCold) 
			{
				logger.Info ("Temperature below lower limit");
				if (switchAbove.State.Value != StateValue.Off) 
				{
					switchAbove.TurnOff ();
				}

				if (switchBelow.State.Value != StateValue.On) {
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
					OptionalData = data.OptionalData
				};
			} else
				return new TemperatureAlarmResponse () {
					NotificationText = configuration.NotificationTextBelow,
					Priority = NotificationPriority.High,
					OptionalData = data.OptionalData
				};
		}



        public void Dispose()
        {
            logger.DebugFormat("Dispose action {0}", this);
            UnSubscribe();
		}
	}




}

