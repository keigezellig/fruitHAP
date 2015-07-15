using System;

namespace FruitHAP.Core.Action
{
    public interface IAction : IDisposable
    {
        void Initialize();
    }
}
