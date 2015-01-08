namespace SensorBinding.Decoders
{
    public interface IDecoder
    {
        bool Decode(byte[] input);
    }
}
