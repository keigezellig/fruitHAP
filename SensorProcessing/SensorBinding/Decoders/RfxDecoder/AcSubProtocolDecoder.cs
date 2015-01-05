using System;
using System.Linq;
using SensorBinding.Pdu;

namespace SensorBinding.Decoders.RfxDecoder
{
    internal class AcSubProtocolDecoder : IDecoder
    {
        public void Decode(byte[] input)
        {
            AcPdu pdu = new AcPdu();
            pdu.DeviceId = BitConverter.ToUInt32(input, 4);
            pdu.UnitCode = input[8];
            pdu.Command = (AcCommand)input[9];
            pdu.Level = input[10];
        }
    }
}