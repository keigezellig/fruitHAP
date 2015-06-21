using System;

namespace FruitHAP.Core.Sensor.Controllers
{
	public class ControllerEventData<TPayload>
	{
		public Direction Direction {get; set;}
		public TPayload Payload { get; set;}
		
	}

	public enum Direction
	{
		FromController,
		ToController
	}

}

