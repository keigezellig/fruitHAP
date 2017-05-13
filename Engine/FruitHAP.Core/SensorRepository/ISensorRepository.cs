using System.Collections.Generic;
using FruitHAP.Core.Sensor;
using System.Collections;
using System;
using System.Reflection;

namespace FruitHAP.Core.SensorRepository
{
    public interface ISensorRepository : IDisposable
    {
        void Initialize();
        IEnumerable<T> FindAllSensorsOfType<T>() where T: ISensor;
        IEnumerable<ISensor> FindAllSensorsOfTypeByTypeName(string typeName);
        T FindSensorOfTypeByName<T>(string name) where T : ISensor;
		IEnumerable<ISensor> GetSensors ();
        ISensor GetSensorByName (string name);
		void SaveSensors(IEnumerable<ISensor> sensors);
        IEnumerable<ISensor> GetSensorsByCategoryName(string category);
        IEnumerable<string> GetSensorCategories();
        IEnumerable<MethodInfo> GetOperationsForSensor(string sensorName);
        MethodInfo GetOperationForSensor(string sensorName, string operationName);
        Type GetSensorValueType(string sensorName);
    }
}
