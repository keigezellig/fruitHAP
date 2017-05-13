using System;
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHAP.Core.Sensor.SensorTypes
{
    public interface IHumiditySensor : IValueSensor
	{
        QuantityValue<String> Humidity { get; }
	}
}

