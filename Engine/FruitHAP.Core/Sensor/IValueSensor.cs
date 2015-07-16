using System;

namespace FruitHAP.Core.Sensor
{
	public interface IValueSensor : ISensor
	{
		object GetValue ();
	}
}

