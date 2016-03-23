using System;
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHAP.Core.Sensor
{
	public class OptionalDataContainer
	{
        private ISensorValueType content;
		
		public ISensorValueType Content 
        { 
            get
            {
                return content;
            }
        }
		public OptionalDataContainer (ISensorValueType content)
		{						
			this.content = content;
		}

		public override string ToString ()
		{
			return string.Format ("[OptionalDataContainer: Content={0}]",  Content);
		}
		
	}
}

