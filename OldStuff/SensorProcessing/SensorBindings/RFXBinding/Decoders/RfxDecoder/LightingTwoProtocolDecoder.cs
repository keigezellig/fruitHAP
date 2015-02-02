using System.Collections.Generic;
using Castle.Core.Logging;

namespace FruitHAP.SensorProcessing.SensorBinding.RfxBinding.Decoders.RfxDecoder
{
    public class LightingTwoProtocolDecoder : BaseDecoder
    {
        private readonly List<IDecoder> subDecoders;

        public LightingTwoProtocolDecoder(List<IDecoder> subDecoders, ILogger logger) : base(logger)
        {
            this.subDecoders = subDecoders;
        }


        protected override bool ExecuteDecode(byte[] input)
        {
            bool decodeResult = true;

            foreach (var subDecoder in subDecoders)
            {
                decodeResult = subDecoder.Decode(input);
                if (decodeResult)
                {
                    break;
                }
            }

            return decodeResult;
        }

        protected override bool CanDecode(byte[] input)
       {
           return input[0] == 0x0B && input[1] == 0x11;
       }
      
    }
}
