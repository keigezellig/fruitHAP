using System;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Core;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHap.StandardActions.Messages.Outbound;
using FruitHAP.Core.Action;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.MQ;

namespace FruitHap.StandardActions
{
	public class EventTriggerAction : IAction
	{
		private readonly ISensorRepository sensorRepository;
		private readonly ILogger logger;
		private readonly IMessageQueueProvider mqProvider;
		private const string ROUTINGKEY = "alerts";

		public EventTriggerAction(ISensorRepository sensorRepository, ILogger logger, IMessageQueueProvider publisher)
		{
			this.sensorRepository = sensorRepository;
			this.logger = logger;
			this.mqProvider = publisher;
		}

		public void Initialize()
		{
			logger.InfoFormat ("Initializing action {0}", this);
			var buttons = sensorRepository.FindAllDevicesOfType<IButton> ();
			var switches = sensorRepository.FindAllDevicesOfType<ISwitch> ();
			foreach (var button in buttons) 
			{
				button.ButtonPressed += Button_ButtonPressed;
			}
			 
			foreach (var @switch in switches) 
			{
				@switch.StateChanged += Switch_StateChanged;
			}

		}

		void Switch_StateChanged (object sender, SwitchEventArgs e)
		{
			var sensorName = (sender as ISensor).Name;
			var sensorMessage = new SensorMessage () {
				TimeStamp = DateTime.Now,
				SensorName = sensorName,
				SensorType = "Switch",
				Data = e.NewState.ToString(),
				DataType = DataType.Event.ToString()
			};
			logger.InfoFormat ("Message sent {0}", sensorMessage);
			mqProvider.Publish (sensorMessage, ROUTINGKEY);
		}

		void Button_ButtonPressed (object sender, EventArgs e)
		{
			var sensorName = (sender as ISensor).Name;
			var sensorMessage = new SensorMessage () {
				TimeStamp = DateTime.Now,
				SensorName = sensorName,
				Data = null,
				SensorType = "Button",
				DataType = DataType.Event.ToString()
			};
			logger.InfoFormat ("Message sent {0}", sensorMessage);
			mqProvider.Publish (sensorMessage, ROUTINGKEY);
		}

	}
}

