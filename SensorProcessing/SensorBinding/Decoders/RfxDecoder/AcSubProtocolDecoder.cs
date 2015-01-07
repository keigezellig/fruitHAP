using System;
using System.Linq;
using SensorBinding.Eventing;
using SensorBinding.Pdu;

namespace SensorBinding.Decoders.RfxDecoder
{
    public class AcSubProtocolDecoder : IDecoder
    {
        private readonly IAcPduPublisher pduPublisher;

        public AcSubProtocolDecoder(IAcPduPublisher pduPublisher)
        {
            this.pduPublisher = pduPublisher;
        }

        public bool Decode(byte[] input)
        {
            if (!IsAcProtocol(input))
            {
                return false;
            }
           
            AcPdu pdu = new AcPdu();
            byte[] deviceBytes = input.Skip(4).Take(4).Reverse().ToArray();
            pdu.DeviceId = BitConverter.ToUInt32(deviceBytes,0);
            pdu.UnitCode = input[8];
            pdu.Command = (AcCommand)input[9];
            pdu.Level = input[10];

            pduPublisher.Publish(pdu);

            return true;
        }

        private bool IsAcProtocol(byte[] input)
        {
            return input[2] == 0x00;
        }

    }
}