using FruitHAP.SensorProcessing.Common.Pdu;
using Microsoft.Practices.Prism.PubSubEvents;

namespace FruitHAP.SensorProcessing.Common.Eventing
{
    public class AcPduAvailable : PubSubEvent<AcPdu>
    {
    }
}
