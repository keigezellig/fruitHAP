namespace SensorProcessing.SensorBinding.RfxBinding.Decoders
{
    public interface IDecoder
    {
        bool Decode(byte[] input);
    }
}
