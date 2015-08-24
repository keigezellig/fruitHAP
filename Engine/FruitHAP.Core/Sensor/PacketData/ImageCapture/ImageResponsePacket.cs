using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.Core.Sensor.PacketData.ImageCapture
{
    public class ImageResponsePacket
    {
        public string Destination { get; set; }
        public byte[] ImageData { get; set; }
    }
}
