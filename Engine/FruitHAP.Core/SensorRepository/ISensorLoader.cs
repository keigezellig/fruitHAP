using System.Collections.Generic;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.SensorRepository
{
    public interface ISensorLoader
    {
        IEnumerable<ISensor> LoadSensors();
    }
}