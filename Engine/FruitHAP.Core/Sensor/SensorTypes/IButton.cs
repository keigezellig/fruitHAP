using System;

namespace FruitHAP.Core.Sensor.SensorTypes
{
    public interface IButton : ISensor
    {
        event EventHandler ButtonPressed;
    }
}
