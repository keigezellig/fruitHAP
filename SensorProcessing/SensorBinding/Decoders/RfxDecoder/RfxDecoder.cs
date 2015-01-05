
namespace SensorBinding.Decoders.RfxDecoder
{
    public class RfxDecoder : ChainedDecoder
    {
        public static int SequenceNumber;
        
        protected override IDecoder DetermineNextDecoder(byte[] input)
        {
            if (input[0] == 0x0B && input[1] == 0x11 && input.Length == 12)
            {
                return new LightningTwoProtocolDecoder();
            }

            return new NullDecoder();
        }

        protected override void DoExtraActions(byte[] input)
        {
            SequenceNumber = input[3];
        }
    }
}
