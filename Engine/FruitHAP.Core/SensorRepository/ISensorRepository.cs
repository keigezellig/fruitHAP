using System.Collections.Generic;
using FruitHAP.Core.Sensor;
using System.Collections;
using System;

namespace FruitHAP.Core.SensorRepository
{
    public interface ISensorRepository
    {
        void Initialize();
        IEnumerable<T> FindAllSensorsOfType<T>() where T: ISensor;
        IEnumerable<ISensor> FindAllSensorsOfTypeByTypeName(string typeName);
        T FindSensorOfTypeByName<T>(string name) where T : ISensor;
		IEnumerable<ISensor> GetSensors ();
		void SaveSensors(IEnumerable<ISensor> sensors);
        IEnumerable<ISensor> GetSensorsByCategoryName(string category);
        IEnumerable<string> GetSensorCategories();
    }
}
