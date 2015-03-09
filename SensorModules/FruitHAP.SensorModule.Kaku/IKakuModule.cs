using System;
using FruitHAP.Core.Sensor;
using FruitHAP.SensorModule.Kaku.Protocol;

namespace FruitHAP.SensorModule.Kaku
{
    public interface IKakuModule : ISensorModule
    {
        event EventHandler<KakuProtocolEventArgs> KakuDataReceived;
		void SendData(KakuProtocolData data);
    }
}