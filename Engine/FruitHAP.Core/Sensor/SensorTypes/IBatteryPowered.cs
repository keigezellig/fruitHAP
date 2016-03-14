using System;

namespace FruitHAP.Core.Sensor.SensorTypes
{
	public interface IBatteryPowered
	{
		double GetPercentOfCharge();
	}
}

