using System;

namespace FruitHAP.Core.Sensor.SensorValueTypes
{
	public class ImageValue : ISensorValueType
	{
		public byte[] ImageData { get; set; }
		public override string ToString ()
		{
			return string.Format ("[ImageValue: ImageData (len)={0}]", ImageData.Length);
		}
		
	}


}

