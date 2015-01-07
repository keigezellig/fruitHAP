using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensorBinding.Decoders
{
    public interface IDecoder
    {
        bool Decode(byte[] input);
    }
}
