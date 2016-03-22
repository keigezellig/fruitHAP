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
using FruitHap.StandardActions.EventNotification.Configuration;



namespace FruitHap.StandardActions.EventNotification
{
	public class EventNotificationAction : IAction
	{
		private readonly ISensorRepository sensorRepository;
		private readonly ILogger logger;
		private readonly IMessageQueueProvider mqProvider;
		private IConfigProvider<EventNotificationActionConfiguration> configurationProvider;
		private const string CONFIG_FILENAME = "eventnotification_action.json";
		private EventNotificationActionConfiguration configuration;
		private IEventBus eventBus;

		public EventNotificationAction(ISensorRepository sensorRepository, 
								  ILogger logger, 
			IConfigProvider<EventNotificationActionConfiguration> configurationProvider, 
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

			bool isAnyActionTriggered = sensorRepository.GetSensors ().Any (sns => this.configuration.Sensors.Contains (sns.Name));
			if (!isAnyActionTriggered) 
			{
				logger.Warn ("This action will never be triggered. If this isn't correct, please check your configuration");
			}

			Subscribe (this.configuration.Sensors);
		}

		void Subscribe (List<string> sensors)
		{
			logger.Debug ("Subscribing to sensor events");
			foreach (var sensor in sensors) 
			{
				eventBus.Subscribe<SensorEventData> (HandleSensorEvent, f => f.Sender.Name.Contains (sensor));
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
				Data = data.OptionalData,
				EventType = data.EventName
			};


			mqProvider.Publish (sensorMessage, configuration.RoutingKey);
		}

        public void Dispose()
        {
            logger.DebugFormat("Dispose action {0}", this);
            UnSubscribe();
		}
	}




}

