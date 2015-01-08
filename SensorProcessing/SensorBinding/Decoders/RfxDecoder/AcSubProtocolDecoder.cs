using System;
using System.Linq;
using SensorBinding.Eventing;
using SensorBinding.Pdu;

namespace SensorBinding.Decoders.RfxDecoder
{
    public class AcSubProtocolDecoder : BaseDecoder
    {
        private readonly IAcPduPublisher pduPublisher;

        public AcSubProtocolDecoder(IAcPduPublisher pduPublisher)
        {
            this.pduPublisher = pduPublisher;
        }

        protected override bool ExecuteDecode(byte[] input)
        {
            AcPdu pdu = new AcPdu();
            byte[] deviceBytes = input.Skip(4).Take(4).Reverse().ToArray();
            pdu.DeviceId = BitConverter.ToUInt32(deviceBytes,0);
            pdu.UnitCode = input[8];
            pdu.Command = (AcCommand)input[9];
            pdu.Level = input[10];

            pduPublisher.Publish(pdu);

            return true;
        }

        protected override bool CanDecode(byte[] input)
        {
            return input[2] == 0x00;
        }
    }
}