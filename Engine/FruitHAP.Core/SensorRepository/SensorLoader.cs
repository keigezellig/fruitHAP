﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;
using FruitHAP.Core.Sensor;
using FruitHAP.Common.Configuration;
using FruitHAP.Core.SensorConfiguration;

namespace FruitHAP.Core.SensorRepository
{
    public class SensorLoader : ISensorLoader
    {
        private readonly IEnumerable<ISensor> prototypes;
        private readonly ILogger logger;
		private ISensorConfigurationRepository sensorConfiguration;

		public SensorLoader(IEnumerable<ISensor> prototypes, ILogger logger, ISensorConfigurationRepository sensorConfiguration )
        {
			this.sensorConfiguration = sensorConfiguration;
            this.prototypes = prototypes;
            this.logger = logger;
        }

        public IEnumerable<ISensor> LoadSensors()
        {
			string sensorFile = ConfigurationManager.AppSettings ["SensorConfigurationFile"] ?? "./sensors.json";
            
            var result = new List<ISensor>();

			List<SensorDefinition> definitions = sensorConfiguration.GetSensorList ();

			foreach (var definition in definitions)
            {
                try
                {
                    ISensor prototype = prototypes.SingleOrDefault(f => f.GetType().Name.Contains(definition.SensorType));

                    if (prototype != null)
                    {
                        var instance = GetInstance(prototype);
                        Dictionary<string, string> parameters = new Dictionary<string, string>(definition.Parameters);
                        parameters["Name"] = definition.Name;
                        parameters["Description"] = definition.Description;
                        instance.Initialize(parameters);
                        result.Add(instance as ISensor);
                    }
                    else
                    {
                        logger.ErrorFormat("Cannot load sensor {0}. Check your configuration!", definition.SensorType);
                    }
                }
                catch (Exception exception)
                {
                    logger.ErrorFormat("Cannot load sensor {0}. Check your configuration! Error message={1}", definition.SensorType, exception.Message);
                }
            }
            return result;
        }

        private ISensorInitializer GetInstance(object prototype)
        {
            var clonableType = prototype as ICloneable;            
            return clonableType.Clone() as ISensorInitializer;
        }
    }
}
