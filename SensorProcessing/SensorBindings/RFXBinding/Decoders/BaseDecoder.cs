namespace SensorProcessing.SensorBinding.RfxBinding.Decoders
{
    public abstract class BaseDecoder : IDecoder
    {
        protected abstract bool CanDecode(byte[] input);
        protected abstract bool ExecuteDecode(byte[] input);
        
        public bool Decode(byte[] input)
        {
            return CanDecode(input) && ExecuteDecode(input);
        }
    }
}
