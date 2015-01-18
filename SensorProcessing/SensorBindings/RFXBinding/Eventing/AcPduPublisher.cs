using System;
using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using SensorProcessing.Common.Eventing;
using SensorProcessing.Common.Pdu;

namespace SensorProcessing.SensorBinding.RfxBinding.Eventing
{
    public class AcPduPublisher : IAcPduPublisher
    {
        private readonly IEventAggregator eventAggregator;
        private readonly ILogger logger;

        public AcPduPublisher(IEventAggregator eventAggregator, ILogger logger)
        {
            this.eventAggregator = eventAggregator;
            this.logger = logger;
        }

        public void Publish(AcPdu pdu)
        {
            logger.InfoFormat("Pdu published {0}",pdu);
            eventAggregator.GetEvent<AcPduAvailable>().Publish(pdu);
        }
    }
}
