using System;
using FruitHAP.Core.Sensor.SensorTypes;
using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Sensor;
using System.Collections.Generic;
using System.Linq;
using FruitHAP.Common.Helpers;
using FruitHAP.Common.EventBus;

namespace FruitHAP.Sensor.Aggregated
{
	public class SwitchWithCameraSensor : IAggregatedSensor, ICloneable
	{
		private ISwitch @switch;
		private ICamera camera;
		private ILogger logger;
		private List<ISensor> inputs;
		private IEventBus eventBus;

		public SwitchWithCameraSensor (IEventBus eventBus, ILogger logger)
		{
			this.eventBus = eventBus;
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
			eventBus.Subscribe<SensorEventData> (OnSwitchStateChanged, f => f.Sender == this.@switch);
		}

		void OnSwitchStateChanged (SensorEventData data)
		{
			var image = this.camera.GetImageAsync ().Result;
			SensorEventData sensorEvent = new SensorEventData () {
				TimeStamp = data.TimeStamp,
				Sender = this,
				EventName = data.EventName,
				OptionalData = image
			};

			eventBus.Publish(sensorEvent);
		}
			
		#endregion

		#region ICloneable implementation

		public object Clone ()
		{			
			return new SwitchWithCameraSensor (eventBus, logger);
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
			eventBus.Unsubscribe<SensorEventData> (OnSwitchStateChanged);
		}
		#endregion
	}
}

