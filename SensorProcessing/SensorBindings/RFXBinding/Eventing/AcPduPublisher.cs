using System;
using Castle.Core.Logging;
using SensorProcessing.Common.Pdu;

namespace SensorProcessing.SensorBinding.RfxBinding.Eventing
{
    public class AcPduPublisher : IAcPduPublisher
    {
        private readonly ILogger logger;

        public AcPduPublisher(ILogger logger)
        {
            this.logger = logger;
        }

        public void Publish(AcPdu pdu)
        {
            logger.Info(pdu.ToString());
        }
    }
}
