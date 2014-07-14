using System;

namespace EventNotifierService
{
	public class EventNotifierServiceException : Exception
	{
        public EventNotifierServiceException(string message) :
            base(message)
        {
        }

        public EventNotifierServiceException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

	}
}

