using System;

namespace FruitHAP.SensorProcessing.Common
{
    public interface IBinding : IDisposable
    {
        void Start();
        void Stop();
    }
}
