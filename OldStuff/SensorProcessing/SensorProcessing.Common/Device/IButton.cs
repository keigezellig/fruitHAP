using System;

namespace FruitHAP.SensorProcessing.Common.Device
{
    public interface IButton : IDevice
    {
        event EventHandler ButtonPressed;
    }
}
