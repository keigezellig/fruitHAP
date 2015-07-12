using System;
using FruitHAP.Core.Sensor.SensorTypes;
using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Sensor;
using System.Collections.Generic;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.SensorTypes;
using System.Linq;

namespace FruitHAP.Sensor.Aggregated.Sensors
{
	public class ButtonWithCameraSensor : IAggregatedSensor, ICloneable
	{
		private IButton button;
		private ICamera camera;
		private ILogger logger;
		private IEventAggregator aggregator;
		private List<ISensor> inputs;

		public ButtonWithCameraSensor (IEventAggregator aggregator, ILogger logger)
		{
			this.aggregator = aggregator;
			this.logger = logger;
		}

		#region ISensor implementation

		public string Name { get; set; }

		public string Description { get; set; }
			

		public List<ISensor> Inputs {
			get 
			{
				return inputs;
			}
		}

		public event EventHandler<ButtonWithCameraSensorEventArgs> DataChanged;



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

			this.button.ButtonPressed += Button_ButtonPressed;
		}

		void Button_ButtonPressed (object sender, EventArgs e)
		{
			var image = this.camera.GetImageAsync ().Result;
			OnDataChanged (image);
		}
		#endregion

		#region ICloneable implementation

		public object Clone ()
		{
			return new ButtonWithCameraSensor (aggregator, logger);
		}

		#endregion

		protected void OnDataChanged (byte[] image)
		{
			var localEvent = DataChanged;
			if (localEvent != null) 
			{
				localEvent.Invoke(this,new ButtonWithCameraSensorEventArgs() { CameraImage = image});
			}
		}




	}
}

