using System;
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHAP.Core.Sensor
{
	public interface IValueSensor : ISensor
	{
		ISensorValueType GetValue ();
		DateTime GetLastUpdateTime();
	}
}

