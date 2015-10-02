using System;
using FruitHAP.Core.Sensor.SensorTypes;
using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Sensor;
using System.Collections.Generic;
using System.Linq;
using FruitHAP.Core.SensorEventPublisher;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Sensor.Aggregated
{
	public class SwitchWithCameraSensor : IAggregatedSensor, ICloneable
	{
		private ISwitch @switch;
		private ICamera camera;
		private ILogger logger;
		private ISensorEventPublisher sensorEventPublisher;
		private List<ISensor> inputs;

		public SwitchWithCameraSensor (ISensorEventPublisher sensorEventPublisher, ILogger logger)
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

			if (inputs.Count != 2) {
				throw new ArgumentException ("This aggregated sensor needs exactly 2 sensor inputs");
			}

			if ((!inputs.Any (input => input is ISwitch)) && (!inputs.Any (input => input is ICamera))) {
				throw new ArgumentException ("This aggregated sensor needs a button type input and a switch type inputs");
			}

			this.inputs = inputs;
			this.@switch = inputs.Single (f => f is ISwitch) as ISwitch;
			this.camera = inputs.Single (f => f is ICamera) as ICamera;
			sensorEventPublisher.Subscribe<SensorEvent> (OnSwitchStateChanged, f => f.Sender == this.@switch);
		}

		void OnSwitchStateChanged (EventData data)
		{
			var image = this.camera.GetImageAsync ().Result;
			if (image != null) {
				sensorEventPublisher.Publish<SensorEvent> (this, Convert.ToBase64String (image));
			} 
			else 
			{
				sensorEventPublisher.Publish<SensorEvent> (this, null);
			}

				
		}
			
		#endregion

		#region ICloneable implementation

		public object Clone ()
		{			
			return new SwitchWithCameraSensor (sensorEventPublisher, logger);
		}

		#endregion

		public override string ToString ()
		{
			return string.Format ("[ButtonWithCameraSensor: Name={0}, Description={1}, Inputs={2}]", Name, Description, string.Join(",",Inputs.Select(i => i.Name)));
		}

		#region IDisposable implementation
		public void Dispose ()
		{
			logger.Debug ("Unsubscribing from events");
			sensorEventPublisher.Unsubscribe<SensorEvent> (OnSwitchStateChanged);
		}
		#endregion
	}
}

