using System;

namespace FruitHAP.Core
{
	public interface ISwitch : IReadOnlySwitch
	{
		void TurnOn();
		void TurnOff();
	}
}

