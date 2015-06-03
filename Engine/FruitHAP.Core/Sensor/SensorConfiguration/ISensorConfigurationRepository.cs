﻿using System;
using System.Collections.Generic;

namespace FruitHAP.Core.SensorConfiguration
{
	public interface ISensorConfigurationRepository
	{
		List<SensorDefinition> GetSensorList();
		SensorDefinition GetSensorDefinition(string sensorName);
		SensorDefinition AddSensorDefinition(string name, string type, Dictionary<string, string> parameters);
		SensorDefinition UpdateSensorDefinition(string name, string type, Dictionary<string, string> parameters);
		List<string> GetSensorTypes();
		void DeleteSensor(string sensorName);
		void DeleteAll();
	}
}

