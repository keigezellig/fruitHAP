using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensorBinding.Decoders.RfxDecoder
{
    internal class LightningTwoProtocolDecoder : ChainedDecoder
    {
        protected override IDecoder DetermineNextDecoder(byte[] input)
        {
            if (input[2] == 0x00)
            {
                return new AcSubProtocolDecoder();
            }

            return new NullDecoder();
        }
    }
}
