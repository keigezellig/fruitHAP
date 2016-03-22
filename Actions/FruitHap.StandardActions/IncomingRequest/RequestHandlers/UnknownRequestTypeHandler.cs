using System;
using FruitHAP.Core.Action;
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHap.StandardActions.IncomingRequest.RequestHandlers
{
	public class UnknownRequestTypeHandler : IRequestHandler
	{
		#region IRequestHandler implementation

		public SensorMessage HandleRequest (SensorMessage request)
		{
			TextValue data = new TextValue () { Text = "Unknown data type in request" };
			return new SensorMessage () {
				TimeStamp = DateTime.Now,
				Data = new FruitHAP.Core.Sensor.OptionalDataContainer(data),
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

