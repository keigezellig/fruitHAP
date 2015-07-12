using System;
using System.Collections.Generic;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Common.Configuration;
using System.Configuration;
using System.IO;
using System.Linq;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Core.SensorPersister
{
	public class SensorPersister : ISensorPersister
	{
		IEnumerable<ISensor> prototypes;
		ILogger logger;
		IConfigProvider<List<SensorConfigurationEntry>> configProvider;
		string sensorFile;

		public SensorPersister (IEnumerable<ISensor> prototypes, ILogger logger, IConfigProvider<List<SensorConfigurationEntry>> configProvider)
		{
			this.configProvider = configProvider;
			this.logger = logger;
			this.prototypes = prototypes;
			sensorFile = ConfigurationManager.AppSettings ["SensorConfigurationFile"] ?? Path.Combine (".", "sensors.json");
		}

		#region ISensorPersister implementation

		public IEnumerable<ISensor> LoadSensors ()
		{
			var result = new List<ISensor> ();
			logger.InfoFormat ("Loading sensors from {0}", sensorFile);
			var configuration = configProvider.LoadConfigFromFile (sensorFile); 
			List<ISensor> nonAggregateSensors = LoadNonAggregateSensors (configuration.Where (f => !f.IsAggegrate));
			List<ISensor> aggregateSensors = LoadAggregateSensors (configuration.Where (f => f.IsAggegrate));
			result.AddRange (nonAggregateSensors);
			result.AddRange (aggregateSensors);

			return result;
		}

		public void SaveSensors (IEnumerable<ISensor> sensorList)
		{
			

			List<SensorConfigurationEntry> entries = new List<SensorConfigurationEntry> ();
			foreach (var sensor in sensorList) 
			{
				string type = sensor.GetType ().Name;
				bool isAggregate = sensor is IAggregatedSensor;

				SensorConfigurationEntry entry = new SensorConfigurationEntry () {
					Type = type,
					IsAggegrate = isAggregate,
					Parameters = sensor
				};

				entries.Add (entry);
			}

			configProvider.SaveConfigToFile (entries, sensorFile);
		}

		#endregion

		List<ISensor> LoadNonAggregateSensors (IEnumerable<SensorConfigurationEntry> configurationEntries)
		{
			var result = new List<ISensor> ();
			foreach (var entry in configurationEntries) 
			{
				ISensor prototype = prototypes.SingleOrDefault (f => f.GetType ().Name.Contains (entry.Type));
				if (prototype == null) {
					logger.WarnFormat ("Ignoring sensor type {0} because it is not supported. Check your sensor configuration ", entry.Type);
				} 
				else 
				{					
					string parametersInJson = entry.Parameters.ToJsonString ();
					object instance = parametersInJson.ParseJsonString (prototype.GetType ());
					logger.InfoFormat ("Loaded sensor {0} with parameters {1}", instance.GetType ().Name, parametersInJson);
					result.Add (instance as ISensor);
				}
			}

			return result;
		}


		List<ISensor> LoadAggregateSensors (IEnumerable<SensorConfigurationEntry> sensors)
		{
			return new List<ISensor> ();
		}
	}
}

