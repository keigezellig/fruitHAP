using System;
using FruitHap.Core.Action;

namespace FruitHap.StandardActions.IncomingRequest.RequestHandlers
{
	public class EmptyRequestHandler : IRequestHandler
	{
		#region IRequestHandler implementation

		public SensorMessage HandleRequest (SensorMessage request)
		{
			return new SensorMessage() {TimeStamp = DateTime.Now, Data = "Invalid request", EventType = RequestDataType.ErrorMessage.ToString()};
		}

		#endregion

		public EmptyRequestHandler ()
		{
		}
	}
}

