using System.Collections.Generic;
using System;
using FruitHAP.Common.Configuration;

namespace FruitHAP.Core.Sensor
{
	public interface ISensor : IDisposable, ICloneable
    {
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }
        string Category { get; }

        void Initialize();
    }
}
