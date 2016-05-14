using System.Collections.Generic;
using System;

namespace FruitHAP.Core.Sensor
{
	public interface ISensor : IDisposable, ICloneable
    {
        string Name { get; }
        string Description { get; }
        string Category { get; }

    }
}
