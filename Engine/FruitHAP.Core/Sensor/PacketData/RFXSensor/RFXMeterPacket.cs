using System;

namespace FruitHAP.Core.Sensor.PacketData.RFXSensor
{
    public class RFXMeterPacket
    {
        public byte SensorId
        {
            get;
            set;
        }

        public uint Value
        {
            get;
            set;
        }

        public byte Level
        {
            get;
            set;
        }

        public RFXMeterPacket()
        {
        }

        public override string ToString()
        {
            return string.Format("[RFXMeterPacket: SensorId={0}, Value={1}, Level={2}]", SensorId, Value, Level);
        }
        
    }
}

