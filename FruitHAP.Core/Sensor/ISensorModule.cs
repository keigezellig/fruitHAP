using System;

namespace FruitHAP.Core.Sensor
{
    public interface ISensorModule : IDisposable
    {
        void Start();
        void Stop();
    }
}
