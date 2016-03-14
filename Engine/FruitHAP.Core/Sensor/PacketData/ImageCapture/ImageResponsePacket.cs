using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.Core.Sensor.PacketData.ImageCapture
{
    public class ImageResponsePacket
    {
        public string DestinationSensor { get; set; }
        public byte[] ImageData { get; set; }

		public override string ToString ()
		{
			return string.Format ("[ImageResponsePacket: DestinationSensor={0}, ImageData={1}]", DestinationSensor, ImageData);
		}
    }
}
