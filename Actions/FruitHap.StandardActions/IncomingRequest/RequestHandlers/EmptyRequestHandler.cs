using System;
using FruitHAP.Core.Action;
using FruitHAP.Core.Sensor.SensorValueTypes;
using FruitHAP.Core.Sensor;

namespace FruitHap.StandardActions.IncomingRequest.RequestHandlers
{
	public class EmptyRequestHandler : IRequestHandler
	{
		#region IRequestHandler implementation

		public SensorMessage HandleRequest (SensorMessage request)
		{
			TextValue data = new TextValue () { Text = "Invalid request" };
			return new SensorMessage() {TimeStamp = DateTime.Now, Data = new OptionalDataContainer(data), EventType = RequestDataType.ErrorMessage.ToString()};
		}

		#endregion

		public EmptyRequestHandler ()
		{
		}
	}
}

