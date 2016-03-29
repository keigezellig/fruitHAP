using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.Core.Sensor.PacketData.General
{
    public class NakPacket<T>
    {
        public T Data { get; set; }
        public NakReason Reason { get; set; }
    }

    public enum NakReason
    {
        Timeout, Failure
    }
}
