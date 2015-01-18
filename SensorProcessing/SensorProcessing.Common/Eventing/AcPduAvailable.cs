using Microsoft.Practices.Prism.PubSubEvents;
using SensorProcessing.Common.Pdu;

namespace SensorProcessing.Common.Eventing
{
    public class AcPduAvailable : PubSubEvent<AcPdu>
    {
    }
}
