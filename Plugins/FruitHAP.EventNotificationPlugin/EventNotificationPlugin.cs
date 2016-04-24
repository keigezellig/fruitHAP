using System;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Core.Plugin;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.MQ;
using FruitHAP.Common.Configuration;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using FruitHAP.Common.EventBus;
using FruitHAP.Plugins.EventNotification.Configuration;



namespace FruitHAP.Plugins.EventNotification
{
	public class EventNotificationPlugin : IPlugin
	{
		private readonly ISensorRepository sensorRepository;
		private readonly ILogger logger;
		private readonly IMessageQueueProvider mqProvider;
		private IConfigProvider<EventNotificationConfiguration> configurationProvider;
        private const string CONFIG_FILENAME = "event_notification.json";
		private EventNotificationConfiguration configuration;
		private IEventBus eventBus;

		public EventNotificationPlugin(ISensorRepository sensorRepository, 
								  ILogger logger, 
			IConfigProvider<EventNotificationConfiguration> configurationProvider, 
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
			};

            			
            mqProvider.Publish (sensorMessage, configuration.RoutingKey);
            logger.InfoFormat("Event published for {0}",data.Sender.Name);
		}

        public void Dispose()
        {
            logger.DebugFormat("Dispose action {0}", this);
            UnSubscribe();
		}
	}




}

