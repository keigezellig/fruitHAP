using System;
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHAP.Core.Sensor
{
	public class OptionalDataContainer
	{
		//public string Type { get;}
		public ISensorValueType Content { get; }

		public OptionalDataContainer (ISensorValueType content)
		{			
			//Type = content.GetType ().Name;
			Content = content;
		}

		public override string ToString ()
		{
			return string.Format ("[OptionalDataContainer: Content={0}]",  Content);
		}
		
	}
}

