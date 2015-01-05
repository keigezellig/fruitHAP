using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensorBinding.Decoders
{
    public interface IDecoder
    {
        void Decode(byte[] input);
    }
}
