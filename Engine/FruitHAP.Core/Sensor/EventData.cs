using System;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.Sensor
{
	public class SensorEventData
	{
		public DateTime TimeStamp {get; set;}
		public ISensor Sender { get; set; }
		public string EventName { get; set; }
		public object OptionalData { get; set; }
	}
}

