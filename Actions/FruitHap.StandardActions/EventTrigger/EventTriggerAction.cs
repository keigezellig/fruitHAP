using System;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Core;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHap.StandardActions.Messages.Outbound;
using FruitHAP.Core.Action;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.MQ;
using FruitHAP.Common.Configuration;
using FruitHap.StandardActions.EventTrigger.Configuration;
using System.Linq;
using System.IO;
using System.Reflection;

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

		public EventTriggerAction(ISensorRepository sensorRepository, ILogger logger, IConfigProvider<EventTriggerActionConfiguration> configurationProvider, IMessageQueueProvider publisher)
		{
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

			var sensors = sensorRepository.GetSensors ().Where (sns => this.configuration.Sensors.Contains (sns.Name));
			if (sensors.Count() == 0) 
			{
				logger.Warn ("This action will never be triggered. If this isn't correct, please check your configuration");
			}
			foreach (var sensor in sensors) 
			{
				if (sensor is IButton) 
				{
					(sensor as IButton).ButtonPressed += Button_ButtonPressed;
				}

				if (sensor is ISwitch) 
				{
					(sensor as ISwitch).StateChanged += Switch_StateChanged;
				}
				logger.InfoFormat ("Sensor {0} will trigger this action", sensor.Name);
			}

		}

		void Switch_StateChanged (object sender, SwitchEventArgs e)
		{
			var sensorName = (sender as ISensor).Name;
			var sensorMessage = new SensorMessage () {
				TimeStamp = DateTime.Now,
				SensorName = sensorName,
				SensorType = (sender as ISensor).GetTypeString(),
				Data = e.NewState.ToString(),
				DataType = DataType.Event.ToString()
			};
			logger.InfoFormat ("Message sent {0}", sensorMessage);
			mqProvider.Publish (sensorMessage, configuration.RoutingKey);
		}

		void Button_ButtonPressed (object sender, EventArgs e)
		{
			var sensorName = (sender as ISensor).Name;
			var sensorMessage = new SensorMessage () {
				TimeStamp = DateTime.Now,
				SensorName = sensorName,
				Data = null,
				SensorType = (sender as ISensor).GetTypeString(),
				DataType = DataType.Event.ToString()
			};
			logger.InfoFormat ("Message sent {0}", sensorMessage);
			mqProvider.Publish (sensorMessage, configuration.RoutingKey);
		}

	}
}

