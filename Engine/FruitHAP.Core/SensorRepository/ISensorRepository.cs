using System.Collections.Generic;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.SensorRepository
{
    public interface ISensorRepository
    {
        void Initialize();
        IEnumerable<T> FindAllDevicesOfType<T>() where T: ISensor;
        T FindDeviceOfTypeByName<T>(string name) where T : ISensor;
    }
}
