using System;
using System.Collections.Generic;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.SensorPersister
{
	public interface ISensorPersister
	{
		IEnumerable<ISensor> LoadSensors();		
        IEnumerable<SensorConfigurationEntry> GetSensorConfiguration();
        IEnumerable<ISensor> GetSensorTypes();
        void SaveConfiguration();
        void AddConfigurationEntry(SensorConfigurationEntry entry);
        void DeleteConfigurationEntry(SensorConfigurationEntry entry);
    }
}

