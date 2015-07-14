using System;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;

namespace FruitHap.StandardActions
{
	public static class Helpers
	{
		public static string GetTypeString (this ISensor sensor)
		{
			if (sensor is ISwitch) 
			{
				return "Switch";
			}

			if (sensor is IButton) 
			{
				return "Button";
			}

			if (sensor is ICamera) 
			{
				return "Camera";
			}

			if (sensor is IAggregatedSensor) 
			{
				return sensor.GetType ().Name;
			}

			throw new NotSupportedException ("Sensor is of a non supported type");
		}
	}
}

