using System;
using System.Linq;
using Castle.Core.Logging;
using SensorProcessing.Common.Extensions;
using SensorProcessing.Common.Pdu;
using SensorProcessing.SensorBinding.RfxBinding.Eventing;

namespace SensorProcessing.SensorBinding.RfxBinding.Decoders.RfxDecoder
{
    public class AcSubProtocolDecoder : BaseDecoder
    {
        private readonly IAcPduPublisher pduPublisher;

        public AcSubProtocolDecoder(IAcPduPublisher pduPublisher, ILogger logger) : base(logger)
        {
            this.pduPublisher = pduPublisher;
        }

        protected override bool ExecuteDecode(byte[] input)
        {
            AcPdu pdu = new AcPdu();
            byte[] deviceBytes = input.Skip(4).Take(4).Reverse().ToArray();
            logger.DebugFormat("{0}",deviceBytes.PrettyPrint());
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