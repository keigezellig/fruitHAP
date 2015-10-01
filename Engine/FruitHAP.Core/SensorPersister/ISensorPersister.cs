using System;
using System.Collections.Generic;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.SensorPersister
{
	public interface ISensorPersister
	{
		IEnumerable<ISensor> LoadSensors();
		void SaveSensors(IEnumerable<ISensor> sensorList);
        IEnumerable<SensorConfigurationEntry> GetSensorConfiguration(IEnumerable<ISensor> sensorList);
    }
}

