using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.SensorRepository
{
    public class SensorRepository : ISensorRepository
    {
        private IEnumerable<ISensor> sensors;

        public SensorRepository(ISensorLoader sensorLoader)
        {
            sensors = sensorLoader.LoadSensors();
        }
        
        public IEnumerable<T> FindAllDevicesOfType<T>() where T : ISensor
        {
            return sensors.OfType<T>();
        }

        public T FindDeviceOfTypeByName<T>(string name) where T : ISensor
        {
            return sensors.OfType<T>().SingleOrDefault(f => f.Name == name);
        }
    }
}
