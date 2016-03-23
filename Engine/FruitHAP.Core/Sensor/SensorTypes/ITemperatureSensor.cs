using System;
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHAP.Core.Sensor.SensorTypes
{
	public interface ITemperatureSensor : IValueSensor
	{
		QuantityValue<TemperatureUnit>  Temperature { get; }
	}
}

