using SensorProcessing.Common;

namespace SensorProcessing.SensorBinding.RfxBinding
{
    public interface IRfxProtocolFactory
    {
        IProtocol CreateRfxProtocol();
    }
}