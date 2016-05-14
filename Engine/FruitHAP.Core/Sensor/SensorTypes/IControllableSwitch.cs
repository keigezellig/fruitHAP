using System;

namespace FruitHAP.Core.Sensor.SensorTypes
{
    public interface IControllableSwitch : ISwitch
    {
        void TurnOn();
        void TurnOff();
    }
}

