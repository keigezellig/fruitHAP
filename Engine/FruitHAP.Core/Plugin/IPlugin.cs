using System;

namespace FruitHAP.Core.Plugin
{
    public interface IPlugin : IDisposable
    {
        void Initialize();
        bool IsEnabled { get; }
        string Version { get; }
        string Name { get; }
        string Description { get; }
    }
}
