using System;

namespace FruitHAP.Core.Sensor.SensorTypes
{
	public interface ISwitch : IReadOnlySwitch
	{
		void TurnOn();
		void TurnOff();
	}
}

