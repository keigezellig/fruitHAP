using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.SensorModule.Kaku.Protocol
{
    public interface IKakuProtocol
    {
        KakuProtocolData Decode(byte[] rawData);
        byte[] Encode(KakuProtocolData protocolData);
    }
}
