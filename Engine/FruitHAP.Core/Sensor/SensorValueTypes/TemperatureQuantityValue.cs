using System;

namespace FruitHAP.Core.Sensor.SensorValueTypes
{
	public class TemperatureQuantity : Quantity<TemperatureUnit>
	{		
	}

	public enum TemperatureUnit
	{
		Unknown, Celsius, Fahrenheit, Kelvin
	}
}

