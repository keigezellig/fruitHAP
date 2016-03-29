using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.Core.Sensor.PacketData.General
{
    public class NakPacket
    {
        public Guid MessageId { get; set; }
        public NakReason Reason { get; set; }
    }
}
