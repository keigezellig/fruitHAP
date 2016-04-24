using System;

namespace FruitHAP.Core.Plugin
{
    public interface IPlugin : IDisposable
    {
        void Initialize();
    }
}
