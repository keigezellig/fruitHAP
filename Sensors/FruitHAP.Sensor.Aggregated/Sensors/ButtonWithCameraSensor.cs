using System;
using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Sensor;
using System.Collections.Generic;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.Sensor.SensorTypes;
using System.Linq;
using FruitHAP.Core.SensorEventPublisher;

namespace FruitHAP.Sensor.Aggregated.Sensors
{
	public class ButtonWithCameraSensor : IAggregatedSensor, ICloneable
	{
		private IButton button;
		private ICamera camera;
		private ILogger logger;
		private List<ISensor> inputs;
		private ISensorEventPublisher sensorEventPublisher;

		public ButtonWithCameraSensor (ISensorEventPublisher sensorEventPublisher, ILogger logger)
		{
			this.sensorEventPublisher = sensorEventPublisher;
			this.logger = logger;
		}

		#region ISensor implementation

		public string Name { get; set; }

		public string Description { get; set; }
        public string Category { get; set; }


        public List<ISensor> Inputs {
			get 
			{
				return inputs;
			}
		}


		public void Initialize (List<ISensor> inputs)
		{

			if (inputs.Count != 2) 
			{
				throw new ArgumentException ("This aggregated sensor needs exactly 2 sensor inputs");
			}

			if ( (!inputs.Any(input => input is IButton)) && (!inputs.Any(input => input is ICamera)) ) 
			{
				throw new ArgumentException ("This aggregated sensor needs a button type input and a camera type inputs");
			}

			this.inputs = inputs;
			this.button = inputs.Single (f => f is IButton) as IButton;
			this.camera = inputs.Single (f => f is ICamera) as ICamera;

			sensorEventPublisher.Subscribe<SensorEvent> (OnButtonPressed, f => f.Sender == this.button);
		}

		void OnButtonPressed (EventData data)
		{
			var image = this.camera.GetImageAsync ().Result;
			sensorEventPublisher.Publish<SensorEvent> (this, image);
		}
		#endregion

		#region ICloneable implementation

		public object Clone ()
		{
			return new ButtonWithCameraSensor (sensorEventPublisher, logger);
		}

		#endregion



		public override string ToString ()
		{
			return string.Format ("[ButtonWithCameraSensor: Name={0}, Description={1}, Inputs={2}]", Name, Description, string.Join(",",Inputs.Select(i => i.Name)));
		}

		public void Dispose ()
		{
			logger.Debug ("Unsubscribing from events");
			sensorEventPublisher.Unsubscribe<SensorEvent> (OnButtonPressed);
		}
	}
}

