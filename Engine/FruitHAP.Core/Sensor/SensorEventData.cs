﻿using System;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.Sensor
{
	public class SensorEventData
	{
		public DateTime TimeStamp {get; set;}
		public ISensor Sender { get; set; }
	    public OptionalDataContainer OptionalData { get; set; }

		public override string ToString ()
		{
			return string.Format ("[SensorEventData: TimeStamp={0}, Sender={1}, OptionalData={2}]", TimeStamp, Sender, OptionalData);
		}
	}
}

