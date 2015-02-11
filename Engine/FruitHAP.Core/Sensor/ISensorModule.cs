using System;

namespace FruitHAP.Core.Sensor
{
    public interface ISensorModule : IDisposable
    {
        string Name { get; }
        void Start();
        void Stop();
		bool IsStarted {get;}
    }
}
