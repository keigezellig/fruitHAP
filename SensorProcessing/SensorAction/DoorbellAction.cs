using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using SensorProcessing.Common;
using SensorProcessing.Common.Eventing;
using SensorProcessing.Common.Pdu;

namespace SensorProcessing.SensorAction
{
    public class DoorbellAction : ISensorAction
    {
        private readonly ILogger logger;
        private IEventAggregator eventAggregator;

        public DoorbellAction(IEventAggregator eventAggregator, ILogger logger)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
            
        }

        private void HandleAcPdu(AcPdu pdu)
        {
            logger.InfoFormat("Handled AcPdu: {0} ",pdu);
        }

        public void Initialize()
        {
            logger.InfoFormat("Initializing action {0}", this);
            this.eventAggregator.GetEvent<AcPduAvailable>().Subscribe(HandleAcPdu);
        }
    }

}
