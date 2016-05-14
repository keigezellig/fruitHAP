using System;

namespace FruitHAP.Core.Sensor.SensorValueTypes
{
	public class OnOffValue : ISensorValueType
	{
		public StateValue Value {get; set;}

		public override string ToString ()
		{
			return string.Format ("[OnOffValue: Value={0}]", Value);
		}
		
	}

	public enum StateValue
	{
		Undefined, Off, On
	}
}

