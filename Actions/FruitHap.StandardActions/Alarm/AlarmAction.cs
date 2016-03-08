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


namespace FruitHap.StandardActions.Alarm
{
	public class AlarmAction : IAction
	{
		private readonly ISensorRepository sensorRepository;
		private readonly ILogger logger;
		private readonly IMessageQueueProvider mqProvider;
		private IConfigProvider<AlarmActionConfiguration> configurationProvider;
		private const string CONFIG_FILENAME = "alarm_action.json";
		private AlarmActionConfiguration configuration;
		private IEventBus eventBus;

		public AlarmAction(ISensorRepository sensorRepository, 
								  ILogger logger, 
								  IConfigProvider<AlarmActionConfiguration> configurationProvider, 
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

			bool isAnyActionTriggered = sensorRepository.GetSensors ().Any (sns => this.configuration.Sensors.Select(g => g.SensorName).Contains (sns.Name));
			if (!isAnyActionTriggered) 
			{
				logger.Warn ("This action will never be triggered. If this isn't correct, please check your configuration");
			}

			Subscribe (this.configuration.Sensors);
		}

		void Subscribe (List<NotificationConfiguration> sensors)
		{
			logger.Debug ("Subscribing to sensor events");
			foreach (var sensor in sensors) 
			{
				eventBus.Subscribe<SensorEventData> (HandleSensorEvent, f => f.Sender.Name.Contains (sensor.SensorName));
			}

		} 

		void UnSubscribe ()
		{			
			eventBus.Unsubscribe<SensorEventData> (HandleSensorEvent);
		}

		void HandleSensorEvent (SensorEventData data)
		{			
			var sensorMessage = new SensorMessage () 
			{
				TimeStamp = DateTime.Now,
				SensorName = data.Sender.Name,
				Data = CreateResponse(data),
				EventType = data.EventName
			};

			mqProvider.Publish (sensorMessage, configuration.RoutingKey);
		}


		private AlarmResponse CreateResponse (SensorEventData data)
		{
			var responseConfig = configuration.Sensors.Single (f => f.SensorName == data.Sender.Name);
			string notificationText = responseConfig.NotificationText;
			if (data.Sender is ITemperatureSensor) 
			{
				notificationText = String.Format (notificationText, (data.Sender as ITemperatureSensor).GetTemperature(), (data.Sender as ITemperatureSensor).GetUnit()); 
			}

			return new AlarmResponse () {
				NotificationText = notificationText,
				Priority = (NotificationPriority)((int)responseConfig.Priority),
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

