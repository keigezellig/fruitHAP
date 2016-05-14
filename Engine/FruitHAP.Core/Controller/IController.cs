using System;

namespace FruitHAP.Core.Controller
{
    public interface IController : IDisposable
    {
        string Name { get; }
        string Description { get; }
        string Version { get; }
        void Start();
        void Stop();
		bool IsStarted {get;}
    }


}
