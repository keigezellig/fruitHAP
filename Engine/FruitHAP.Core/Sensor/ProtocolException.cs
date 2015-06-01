using System;

namespace FruitHAP.Core.Sensor
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

