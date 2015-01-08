using System;
using System.Collections.Generic;

namespace SensorBinding.Decoders.RfxDecoder
{
    public class RfxProtocol
    {
        private List<IDecoder> packageDecoders;

        public RfxProtocol(List<IDecoder> packageDecoders)
        {
            this.packageDecoders = packageDecoders;
        }

        public void Process(byte[] input)
        {
            if (input != null && input.Length > 0)
            {
                bool decodeResult = false;
                foreach (var packageDecoder in packageDecoders)
                {
                    decodeResult = packageDecoder.Decode(input);
                }
                if (!decodeResult)
                {
                    Console.WriteLine("Cannot decode input");
                }
            }           
        }
    }
}
