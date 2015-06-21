using System;

namespace FruitHAP.Core.Sensor.Controller
{
	public interface IControllerPacketHandler
	{
		void Handle(byte[] data);
	}
}

