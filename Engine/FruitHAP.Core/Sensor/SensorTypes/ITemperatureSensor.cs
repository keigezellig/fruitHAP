using System;

namespace FruitHAP.Core.Sensor.SensorTypes
{
	public interface ITemperatureSensor : IValueSensor
	{
		double GetTemperature();
		TemperatureUnit GetUnit();
	}

	public enum TemperatureUnit
	{
		Celsius, Kelvin, Fahrenheit
	}
}

