using System;

namespace FruitHAP.Common
{
	public class ValidationException : Exception
	{
		public ValidationException ()
		{
		}
		

		public ValidationException (string message) : base (message)
		{
		}
		

		public ValidationException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
		}
		

		public ValidationException (string message, Exception innerException) : base (message, innerException)
		{
		}

	}
}

