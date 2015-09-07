using FruitHAP.Core.Controller;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.Core.Sensor.PacketData.ImageCapture
{
    public class ImageResponsePacketEvent : PubSubEvent<ControllerEventData<ImageResponsePacket>>
    {
    }
}
