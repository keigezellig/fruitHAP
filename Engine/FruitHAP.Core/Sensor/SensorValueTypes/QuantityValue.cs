using System;

namespace FruitHAP.Core.Sensor.SensorValueTypes
{
	public class QuantityValue<T> : ISensorValueType
	{
		public double Value { get; set; }
		public T Unit { get; set; }

		public override string ToString ()
		{
			return string.Format ("[QuantityValue: Value={0}, Unit={1}]", Value, Unit);
		}
		
	}
}

