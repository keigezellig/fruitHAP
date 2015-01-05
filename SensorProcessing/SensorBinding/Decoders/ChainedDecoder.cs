using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SensorBinding.Decoders
{
    public abstract class ChainedDecoder : IDecoder
    {
        protected abstract IDecoder DetermineNextDecoder(byte[] input);

        protected virtual void DoExtraActions(byte[] input)
        {
            
        }
        
        public void Decode(byte[] input)
        {
            DoExtraActions(input);

            var successor = DetermineNextDecoder(input);

            if (successor != null)
            {
                successor.Decode(input);
            }
            else
            {
                throw new InvalidOperationException("No successor decoder defined");
            }
         }
    }
}
