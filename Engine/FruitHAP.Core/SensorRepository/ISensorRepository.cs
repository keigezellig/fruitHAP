
using System.Collections.Generic;

namespace FruitHAP.Core.Sensor
{
    public interface ISensorRepository
    {
        void Initialize();
        IEnumerable<T> FindAllDevicesOfType<T>() where T: ISensor;
        T FindDeviceOfTypeByName<T>(string name) where T : ISensor;
    }
}
