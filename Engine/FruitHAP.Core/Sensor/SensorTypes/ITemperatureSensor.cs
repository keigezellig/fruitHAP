﻿using System;
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHAP.Core.Sensor.SensorTypes
{
    public interface IHumiditySensor : IValueSensor
	{
        NumberValue Humidity { get; }
	}
}

