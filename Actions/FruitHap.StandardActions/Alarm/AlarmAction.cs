using System;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Core;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Core.Action;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.MQ;
using FruitHAP.Common.Configuration;
using FruitHap.StandardActions.EventTrigger.Configuration;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using FruitHAP.Common.EventBus;


namespace FruitHap.StandardActions.Alarm
{
	public class AlarmAction : IAction
	{
		private readonly ISensorRepository sensorRepository;
		private readonly ILogger logger;
		private readonly IMessageQueueProvider mqProvider;
		private IConfigProvider<EventTriggerActionConfiguration> configurationProvider;
		private const string CONFIG_FILENAME = "event_trigger_action.json";
		private EventTriggerActionConfiguration configuration;
		private IEventBus eventBus;

		public AlarmAction(ISensorRepository sensorRepository, 
								  ILogger logger, IConfigProvider<EventTriggerActionConfiguration> configurationProvider, 
								  IMessageQueueProvider publisher,
								 IEventBus eventBus)
		{
			this.eventBus = eventBus;
			this.sensorRepository = sensorRepository;
			this.logger = logger;
			this.mqProvider = publisher;
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

		void Subscribe (List<EventNotificationConfiguration> sensors)
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
			logger.Info ("Message sent to MQ");
			logger.DebugFormat ("Message sent {0}", sensorMessage);
			mqProvider.Publish (sensorMessage, configuration.RoutingKey);
		}


		private EventTriggerResponse CreateResponse (SensorEventData data)
		{
			var responseConfig = configuration.Sensors.Single (f => f.SensorName == data.Sender.Name);
			return new EventTriggerResponse () {
				NotificationText = responseConfig.NotificationText,
				Priority = (NotificationPriority)((int)responseConfig.Priority),
				OptionalData = new DataContents(data.OptionalData)
			};
		}



        public void Dispose()
        {
            logger.DebugFormat("Dispose action {0}", this);
            UnSubscribe();
		}
	}


	public class EventTriggerResponse
	{		
		public string NotificationText { get; set;}
		public NotificationPriority Priority {get; set;}
		public object OptionalData {get; set;}
	}

	public enum NotificationPriority
	{
		Low, Medium, High
	}

}

