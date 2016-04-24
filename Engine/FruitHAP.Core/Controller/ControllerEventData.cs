using System;

namespace FruitHAP.Core.Controller
{
	public class ControllerEventData<TPayload>
	{
        public Direction Direction {get; set;}
		public TPayload Payload { get; set;}

		public override string ToString ()
		{
            return string.Format ("[ControllerEventData: Direction={0}, Payload={1}", Direction, Payload);
		}
		
	}

	public enum Direction
	{
		FromController,
		ToController
	}

}

