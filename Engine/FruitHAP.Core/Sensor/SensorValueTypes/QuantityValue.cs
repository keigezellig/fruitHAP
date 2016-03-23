using System;

namespace FruitHAP.Core.Sensor.SensorValueTypes
{
	public class QuantityValue<T> : ISensorValueType
	{
		public Quantity<T> Value {get; set;}


		public override string ToString ()
		{
			return string.Format ("[QuantityValue: Value={0}]", Value);
		}
		
	}
}

