using System;
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHAP.Core.Sensor
{
	public class OptionalDataContainer
	{
        private object content;
		
        public object Content 
        { 
            get
            {
                return content;
            }
        }

		public String TypeName 
		{ 
			get
			{
				return content.GetType().Name;
			}
		}
		public OptionalDataContainer (object content)
		{						
			this.content = content;
		}

		public override string ToString ()
		{
			return string.Format ("[OptionalDataContainer: Content={0}]",  Content);
		}
		
	}
}

