using System.Collections.Generic;
using FruitHAP.SensorProcessing.Common.Device;

namespace FruitHAP.SensorProcessing.Common
{
    public interface IDeviceRepository
    {
        List<T> FindAllDevicesOfType<T>() where T: IDevice;
        T FindDeviceOfTypeByName<T>(string name) where T : IDevice;
        void LoadDevices();
    }
}
