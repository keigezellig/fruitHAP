using FruitHAP.SensorProcessing.Common;

namespace FruitHAP.SensorProcessing.SensorBinding.RfxBinding
{
    public interface IRfxProtocolFactory
    {
        IProtocol CreateRfxProtocol();
    }
}