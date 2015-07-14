using System;
using FruitHAP.Core.Sensor.SensorTypes;
using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Sensor;
using System.Collections.Generic;
using System.Linq;

namespace FruitHAP.Sensor.Aggregated
{
	public class SwitchWithCameraSensor : ISwitchWithCameraSensor, ICloneable
	{
		private ISwitch @switch;
		private ICamera camera;
		private ILogger logger;
		private IEventAggregator aggregator;
		private List<ISensor> inputs;

		public SwitchWithCameraSensor (IEventAggregator aggregator, ILogger logger)
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

		public event EventHandler<CameraImageEventArgs> DataChanged;



		public void Initialize (List<ISensor> inputs)
		{

			if (inputs.Count != 2) 
			{
				throw new ArgumentException ("This aggregated sensor needs exactly 2 sensor inputs");
			}

			if ( (!inputs.Any(input => input is ISwitch)) && (!inputs.Any(input => input is ICamera)) ) 
			{
				throw new ArgumentException ("This aggregated sensor needs a button type input and a switch type inputs");
			}

			this.inputs = inputs;
			this.@switch = inputs.Single (f => f is ISwitch) as ISwitch;
			this.camera = inputs.Single (f => f is ICamera) as ICamera;

			this.@switch.StateChanged += Switch_StateChanged;
		}

		void Switch_StateChanged (object sender, SwitchEventArgs e)
		{
			var image = this.camera.GetImageAsync ().Result;
			OnDataChanged (image);
		}
			
		#endregion

		#region ICloneable implementation

		public object Clone ()
		{
			return new SwitchWithCameraSensor (aggregator, logger);
		}

		#endregion

		protected void OnDataChanged (byte[] image)
		{
			var localEvent = DataChanged;
			if (localEvent != null) 
			{
				localEvent.Invoke(this,new CameraImageEventArgs() { CameraImage = image});
			}
		}

		public override string ToString ()
		{
			return string.Format ("[ButtonWithCameraSensor: Name={0}, Description={1}, Inputs={2}]", Name, Description, string.Join(",",Inputs.Select(i => i.Name)));
		}

	}
}

