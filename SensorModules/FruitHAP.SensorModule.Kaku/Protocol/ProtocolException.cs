using System;

namespace FruitHAP.SensorModule.Kaku.Protocol
{
    public class ProtocolException : Exception
    {
        public ProtocolException()
        {
        }

        public ProtocolException(string message) : base(message)
        {
        }

        public ProtocolException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}