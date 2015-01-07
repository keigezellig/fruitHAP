using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SensorBinding.Pdu;

namespace SensorBinding.Eventing
{
    public interface IAcPduPublisher : IPduPublisher<AcPdu>
    {
    }
}
