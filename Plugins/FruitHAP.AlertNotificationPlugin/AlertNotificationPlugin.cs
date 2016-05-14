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
using FruitHAP.Plugins.AlertNotification.Configuration;


namespace FruitHAP.Plugins.AlertNotification
{
    public class AlertNotificationPlugin : BasePluginWithConfiguration<AlertNotificationConfiguration>
	{
		private readonly ISensorRepository sensorRepository;
		private readonly IMessageQueueProvider mqProvider;
		private const string CONFIG_FILENAME = "alert_notification.json";
		private IEventBus eventBus;

		public AlertNotificationPlugin(ISensorRepository sensorRepository, 
								  ILogger logger, 
								  IConfigProvider<AlertNotificationConfiguration> configurationProvider, 
								  IMessageQueueProvider mqProvider,
            IEventBus eventBus) : base(logger,configurationProvider)
		{
			this.eventBus = eventBus;
			this.sensorRepository = sensorRepository;
			this.mqProvider = mqProvider;
		}

        #region implemented abstract members of BasePluginWithConfiguration

        protected override string GetConfigurationFileName()
        {
            return CONFIG_FILENAME;
        }

        protected override void InitializePlugin()
        {
            bool isAnyActionTriggered = sensorRepository.GetSensors ().Any (sns => this.configuration.Sensors.Select(g => g.SensorName).Contains (sns.Name));
            if (!isAnyActionTriggered) 
            {
                logger.Warn ("This action will never be triggered. If this isn't correct, please check your configuration");
            }

            Subscribe (this.configuration.Sensors);
        }

        protected override void CleanUpPlugin()
        {
            UnSubscribe();
        }

        #endregion


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
			};

			mqProvider.Publish (sensorMessage, configuration.RoutingKey);
		}


		private AlertResponse CreateResponse (SensorEventData data)
		{
			var responseConfig = configuration.Sensors.Single (f => f.SensorName == data.Sender.Name);
			string notificationText = responseConfig.NotificationText;
			return new AlertResponse () {
				NotificationText = notificationText,
				Priority = (NotificationPriority)((int)responseConfig.Priority),
				OptionalData = data.OptionalData
			};
		}
	}




}

