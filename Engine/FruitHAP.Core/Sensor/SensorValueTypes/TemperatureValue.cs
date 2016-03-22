using System;

namespace FruitHAP.Core.Sensor.SensorValueTypes
{
	public class TemperatureValue : QuantityValue<TemperatureUnit>
	{		
	}

	public enum TemperatureUnit
	{
		Unknown, Celsius, Fahrenheit, Kelvin
	}
}

