using System;
using FruitHap.Core.Action;

namespace FruitHap.StandardActions.IncomingRequest.RequestHandlers
{
	public class UnknownRequestTypeHandler : IRequestHandler
	{
		#region IRequestHandler implementation

		public SensorMessage HandleRequest (SensorMessage request)
		{
			return new SensorMessage () {
				TimeStamp = DateTime.Now,
				Data = "Unknown data type in request",
				SensorName = request.SensorName,
				EventType = RequestDataType.ErrorMessage.ToString ()
			};
		}

		#endregion

		public UnknownRequestTypeHandler ()
		{
		}
	}
}

