using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SensorProcessing.Common.Device;

namespace SensorProcessing.Common
{
    public interface IDeviceRepository
    {
        List<T> FindAllDevicesOfType<T>() where T: IDevice;
        T FindDeviceOfTypeByName<T>(string name) where T : IDevice;
        void LoadDevices();
    }
}
