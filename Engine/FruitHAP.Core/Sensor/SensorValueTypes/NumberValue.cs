﻿using System;

namespace FruitHAP.Core.Sensor.SensorValueTypes
{
    public class NumberValue : ISensorValueType
	{
		public double Value { get; set; }

		public override string ToString ()
		{
			return string.Format ("[NumberValue: Value={0}]", Value);
		}
		
	}
}

