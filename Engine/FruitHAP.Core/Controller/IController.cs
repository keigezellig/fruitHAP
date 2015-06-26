using System;

namespace FruitHAP.Core.Controller
{
    public interface IController : IDisposable
    {
        string Name { get; }
        void Start();
        void Stop();
		bool IsStarted {get;}
    }


}
