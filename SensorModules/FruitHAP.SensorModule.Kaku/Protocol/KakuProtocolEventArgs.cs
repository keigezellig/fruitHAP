using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.SensorModule.Kaku.Protocol
{
    public class KakuProtocolEventArgs : EventArgs
    {
        public KakuProtocolData Data { get; set; }
    }
}
