using System;

namespace FruitHAP.SensorProcessing.Common
{
    public interface ISensorModule : IDisposable
    {
        void Start();
        void Stop();
    }
}
