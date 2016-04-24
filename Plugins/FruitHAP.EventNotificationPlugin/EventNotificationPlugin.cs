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
    public class EventNotificationPlugin : BasePluginWithConfiguration<EventNotificationConfiguration>
    {
        private readonly ISensorRepository sensorRepository;
        private readonly IMessageQueueProvider mqProvider;
        private IEventBus eventBus;

        public EventNotificationPlugin(IConfigProvider<EventNotificationConfiguration> configurationProvider, ILogger logger,     
                                       ISensorRepository sensorRepository, 
                                       IMessageQueueProvider mqProvider,
                                       IEventBus eventBus)
            : base(logger, configurationProvider)
        {
            this.eventBus = eventBus;
            this.sensorRepository = sensorRepository;			
            this.mqProvider = mqProvider;			
        }

        #region implemented abstract members of BasePluginWithConfiguration

        protected override string GetConfigurationFileName()
        {
            return Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), "event_notification.json");
        }

        protected override void InitializePlugin()
        {
            Subscribe(this.sensorRepository.GetSensors().Select(f => f.Name).ToList());
        }

        protected override void CleanUpPlugin()
        {
            UnSubscribe();
        }

        #endregion

        void Subscribe(List<string> sensors)
        {
            logger.Debug("Subscribing to sensor events");
            foreach (var sensor in sensors)
            {
                eventBus.Subscribe<SensorEventData>(HandleSensorEvent, f => f.Sender.Name.Contains(sensor));
            }

        }

        void UnSubscribe()
        {			
            eventBus.Unsubscribe<SensorEventData>(HandleSensorEvent);
        }

        void HandleSensorEvent(SensorEventData data)
        {			
            var sensorMessage = new SensorMessage()
            {
                TimeStamp = DateTime.Now,
                SensorName = data.Sender.Name,
                Data = data.OptionalData,
            };

            			
            mqProvider.Publish(sensorMessage, configuration.RoutingKey);
            logger.InfoFormat("Event published for {0}", data.Sender.Name);
        }

    }

}

