using System;

namespace FruitHAP.Core.Controller
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

