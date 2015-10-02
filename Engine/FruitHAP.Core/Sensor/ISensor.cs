using System.Collections.Generic;
using System;

namespace FruitHAP.Core.Sensor
{
	public interface ISensor : ICloneable
    {
        string Name { get; }
        string Description { get; }
        string Category { get; }

    }
}
