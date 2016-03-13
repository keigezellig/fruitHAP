using System;

namespace FruitHAP.Core.Sensor.SensorTypes
{
	public interface ITemperatureSensor : IValueSensor
	{
		TemperatureValue GetTemperature();
	}

	public enum TemperatureUnit
	{
		Celsius, Kelvin, Fahrenheit
	}

	public class TemperatureValue
	{
		public double Temperature { get; set; }
		public TemperatureUnit Unit { get; set; }
	}
}

