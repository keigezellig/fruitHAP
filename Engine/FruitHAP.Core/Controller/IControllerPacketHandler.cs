using System;

namespace FruitHAP.Core.Controller
{
	public interface IControllerPacketHandler
	{
		void Handle(byte[] data);
	}
}

