using System.Collections.Generic;

namespace FruitHAP.Core.Sensor
{
    public interface ISensorInitializer
    {
        void Initialize(Dictionary<string, string> parameters);
    }
}