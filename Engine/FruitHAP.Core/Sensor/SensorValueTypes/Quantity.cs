using System;

namespace FruitHAP.Core.Sensor.SensorValueTypes
{
	public class Quantity<T>
	{
		public double Value { get; set; }
		public T Unit { get; set; }
        public string UnitString
        {
            get
            {
                return Unit.ToString();
            }
        }
		public String QuantityType
		{ 
			get
			{
				return GetType().Name;
			}
		}

		public override string ToString ()
		{
			return string.Format ("[QuantityValue: Type={2}, Value={0}, Unit={1}]", Value, Unit, QuantityType);
		}
	}
}

