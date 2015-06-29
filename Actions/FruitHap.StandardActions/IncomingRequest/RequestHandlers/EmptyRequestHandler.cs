using System;
using FruitHap.StandardActions.Messages.Outbound;

namespace FruitHap.StandardActions.IncomingRequest.RequestHandlers
{
	public class EmptyRequestHandler : IRequestHandler
	{
		#region IRequestHandler implementation

		public SensorMessage HandleRequest (SensorMessage request)
		{
			return new SensorMessage() {TimeStamp = DateTime.Now, Data = "Invalid request", DataType = DataType.ErrorMessage.ToString()};
		}

		#endregion

		public EmptyRequestHandler ()
		{
		}
	}
}

