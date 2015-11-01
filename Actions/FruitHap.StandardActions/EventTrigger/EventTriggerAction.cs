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
using Microsoft.Practices.Prism.PubSubEvents;
using System.Collections.Generic;
using FruitHAP.Core.SensorEventPublisher;


namespace FruitHap.StandardActions.EventTrigger
{
	public class EventTriggerAction : IAction
	{
		private readonly ISensorRepository sensorRepository;
		private readonly ILogger logger;
		private readonly IMessageQueueProvider mqProvider;
		private IConfigProvider<EventTriggerActionConfiguration> configurationProvider;
		private const string CONFIG_FILENAME = "event_trigger_action.json";
		private EventTriggerActionConfiguration configuration;

		private List<SubscriptionToken> tokens;


		private ISensorEventPublisher eventPublisher;

		public EventTriggerAction(ISensorRepository sensorRepository, 
								  ILogger logger, IConfigProvider<EventTriggerActionConfiguration> configurationProvider, 
								  IMessageQueueProvider publisher,
			ISensorEventPublisher eventPublisher)
		{
			this.eventPublisher = eventPublisher;
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

			bool isAnyActionTriggered = sensorRepository.GetSensors ().Any (sns => this.configuration.Sensors.Contains (sns.Name));
			if (!isAnyActionTriggered) 
			{
				logger.Warn ("This action will never be triggered. If this isn't correct, please check your configuration");
			}

			Subscribe (this.configuration.Sensors);
		}

		 void Subscribe (List<string> sensorNames)
		{
			tokens = new List<SubscriptionToken> ();
			logger.Debug ("Subscribing to sensor events");
			foreach (string name in sensorNames) 
			{
				tokens.Add (eventPublisher.SubscribeWithToken<SensorEvent> (HandleSensorMessage, f => f.Sender.Name == name));
			}

		} 

		void UnSubscribe ()
		{
			logger.Debug ("Unsubscribing from sensor events");
			foreach (var token in tokens) 
			{
				eventPublisher.Unsubscribe<SensorEvent> (token);
			}

		}

		void HandleSensorMessage (EventData data)
		{			
			var sensorMessage = new SensorMessage () 
			{
				TimeStamp = DateTime.Now,
				SensorName = data.Sender.Name,
				Data = data.OptionalData,
				EventType = data.EventName
			};
			logger.Info ("Message sent to MQ");
			logger.DebugFormat ("Message sent {0}", sensorMessage);
			mqProvider.Publish (sensorMessage, configuration.RoutingKey);
		}


        public void Dispose()
        {
            logger.DebugFormat("Dispose action {0}", this);
            if (tokens != null)
            {
                UnSubscribe();
            }

		}
	}
}

