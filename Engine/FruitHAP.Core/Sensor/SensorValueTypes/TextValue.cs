using System;

namespace FruitHAP.Core.Sensor.SensorValueTypes
{
	public class TextValue : ISensorValueType
	{
		public string Text { get; set; }

		public override string ToString ()
		{
			return string.Format ("[TextValue: Text={0}]", Text);
		}
		
	}
}

