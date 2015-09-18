using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.Core.Sensor.PacketData.ImageCapture
{
    public class ImageRequestPacket
    {
        public string Sender { get; set; }
        public string Uri { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Resolution { get; set; }
    }
}
