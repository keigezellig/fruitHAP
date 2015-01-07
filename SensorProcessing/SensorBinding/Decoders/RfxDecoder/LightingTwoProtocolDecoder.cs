using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensorBinding.Decoders.RfxDecoder
{
    public class LightingTwoProtocolDecoder : IDecoder
    {
        private readonly List<IDecoder> subDecoders;

        public LightingTwoProtocolDecoder(List<IDecoder> subDecoders)
        {
            this.subDecoders = subDecoders;
        }


        public bool Decode(byte[] input)
        {
            bool decodeResult = true;
            if (!IsLightingTwoProtocol(input))
           {
               return false;
           }

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

       private bool IsLightingTwoProtocol(byte[] input)
       {
           return input[0] == 0x0B && input[1] == 0x11;
       }
    }
}
