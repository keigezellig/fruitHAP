using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensorBinding.Decoders
{
    public class NullDecoder : IDecoder
    {
        public void Decode(byte[] input)
        {
            Console.WriteLine("Cannot decode the input");
        }
    }
}
