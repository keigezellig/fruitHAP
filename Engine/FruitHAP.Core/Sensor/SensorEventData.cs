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

		public override string ToString ()
		{
			return string.Format ("[SensorEventData: TimeStamp={0}, Sender={1}, EventName={2}, OptionalData={3}]", TimeStamp, Sender, EventName, OptionalData);
		}
	}
}

