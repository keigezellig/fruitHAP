using System;
using FruitHAP.Core.Sensor.SensorTypes;
using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Sensor;
using System.Collections.Generic;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.SensorTypes;

namespace FruitHAP.Sensor.Aggregated.Sensors
{
	public class ButtonWithCameraSensor : IButtonWithCameraSensor, ICloneable
	{
		private IButton button;
		private ICamera camera;
		private ILogger logger;
		private IEventAggregator aggregator;
		private ISensorRepository repository;
		private string name;	
		private string description;

		public ButtonWithCameraSensor (IEventAggregator aggregator, ILogger logger, ISensorRepository repository)
		{
			this.aggregator = aggregator;
			this.logger = logger;
			this.repository = repository;
		}

		#region ISensor implementation

		public string Name {
			get 
			{
				return this.name;
			}
		}

		public string Description {
			get 
			{
				return this.description;
			}
		}

		public event EventHandler<ButtonWithCameraSensorEventArgs> DataChanged;



		public void Initialize (Dictionary<string, string> parameters)
		{
			this.name = parameters ["Name"];
			this.description = parameters ["Description"];
			this.button = repository.FindSensorOfTypeByName<IButton> (parameters ["ButtonName"]);
			this.camera = repository.FindSensorOfTypeByName<ICamera> (parameters ["CameraName"]);
			if (this.button == null) 
			{
				throw new Exception (string.Format ("No button with name {0} is defined", parameters ["ButtonName"]));
			}
			if (this.camera == null) 
			{
				throw new Exception (string.Format ("No camera with name {0} is defined", parameters ["CameraName"]));
			}

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
			return new ButtonWithCameraSensor (aggregator, logger, repository);
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

