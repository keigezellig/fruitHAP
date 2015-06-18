using System;

namespace FruitHAP.Core.Sensor
{
    public interface ISensorController : IDisposable
    {
        string Name { get; }
        void Start();
        void Stop();
		bool IsStarted {get;}
    }


}
