using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.SensorModule.Kaku.Protocol
{
    public class KakuProtocol : IKakuProtocol
    {
        public KakuProtocolData Decode(byte[] rawData)
        {
            throw new NotImplementedException();
        }

        public byte[] Encode(KakuProtocolData protocolData)
        {
            throw new NotImplementedException();
        }
    }
}
