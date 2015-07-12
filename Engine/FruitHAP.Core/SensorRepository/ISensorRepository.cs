using System.Collections.Generic;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.SensorRepository
{
    public interface ISensorRepository
    {
        void Initialize();
        IEnumerable<T> FindAllSensorsOfType<T>() where T: ISensor;
        T FindSensorOfTypeByName<T>(string name) where T : ISensor;
		IEnumerable<ISensor> GetSensors ();
		void SaveSensors(IEnumerable<ISensor> sensors);
    }
}
