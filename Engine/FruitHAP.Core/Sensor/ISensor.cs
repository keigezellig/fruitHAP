using System.Collections.Generic;

namespace FruitHAP.Core.Sensor
{
    public interface ISensor
    {
        string Name { get; }
        string Description { get; }
		void Initialize(Dictionary<string, string> parameters);

    }
}
