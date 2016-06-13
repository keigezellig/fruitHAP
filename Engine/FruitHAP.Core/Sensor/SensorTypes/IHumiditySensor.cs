using System;
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHAP.Core.Sensor.SensorTypes
{
    public interface IHumiditySensor : IValueSensor
	{
        PercentageQuantity Humidity { get; }
	}
}

