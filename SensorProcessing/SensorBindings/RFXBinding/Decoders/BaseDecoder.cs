using Castle.Core.Logging;

namespace SensorProcessing.SensorBinding.RfxBinding.Decoders
{
    public abstract class BaseDecoder : IDecoder
    {
        protected readonly ILogger logger;
        protected abstract bool CanDecode(byte[] input);
        protected abstract bool ExecuteDecode(byte[] input);

        public BaseDecoder(ILogger logger)
        {
            this.logger = logger;
        }

        public bool Decode(byte[] input)
        {
            return CanDecode(input) && ExecuteDecode(input);
        }
    }
}
