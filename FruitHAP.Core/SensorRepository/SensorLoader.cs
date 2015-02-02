using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.SensorRepository
{
    public class SensorLoader : ISensorLoader
    {
        private readonly IEnumerable<ISensor> prototypes;
        private readonly ILogger logger;

        public SensorLoader(IEnumerable<ISensor> prototypes, ILogger logger )
        {
            this.prototypes = prototypes;
            this.logger = logger;
        }

        public IEnumerable<ISensor> LoadSensors()
        {
            string sensorFile = ConfigurationManager.AppSettings["SensorFile"] ??
                                                  Path.Combine(".", "sensors.xml");

            var result = new List<ISensor>();

            List<SensorDefinition> definitions = XmlSerializerHelper.Deserialize<List<SensorDefinition>>(sensorFile);
            foreach (var definition in definitions)
            {
                ISensor prototype = prototypes.SingleOrDefault(f => f.GetType().Name.Contains(definition.SensorType));

                if (prototype != null)
                {
                    var instance = GetInstance(prototype.GetType());
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
            return result;
        }

        private ISensorInitializer GetInstance(Type type)
        {
            return Activator.CreateInstance(type) as ISensorInitializer;
        }
    }
}
