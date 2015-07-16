using System;

namespace FruitHAP.Core.Sensor.SensorTypes
{
	public interface ITwoWaySwitch : ISwitch
	{
		void TurnOn();
		void TurnOff();
	}
}

