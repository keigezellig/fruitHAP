using System;
using FruitHAP.Common.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace FruitHAP.Core.SensorConfiguration
{
	public class SensorConfigurationRepository : ISensorConfigurationRepository
	{
		private IConfigProvider<List<SensorDefinition>> storageProvider;
		private List<SensorDefinition> definitions;
		private string sensorFile;

		public SensorConfigurationRepository (IConfigProvider<List<SensorDefinition>> storageProvider)
		{
			this.storageProvider = storageProvider;
			sensorFile = ConfigurationManager.AppSettings["SensorFile"] ??
				Path.Combine(".", "sensors.json");
		}

		#region ISensorConfigurationRepository implementation

		public System.Collections.Generic.List<SensorDefinition> GetSensorList ()
		{
			var sensorList = storageProvider.LoadConfigFromFile (sensorFile);
			return sensorList;
		}

		public SensorDefinition GetSensorDefinition (string sensorName)
		{
			var sensorList = storageProvider.LoadConfigFromFile (sensorFile);
			return sensorList.SingleOrDefault (f => f.Name == sensorName);

		}

		public List<string> GetSensorTypes ()
		{
			var sensorList = storageProvider.LoadConfigFromFile (sensorFile);
			return sensorList.Select (f => f.SensorType).Distinct ().ToList ();
		}

		public SensorDefinition AddSensorDefinition (string name, string type, System.Collections.Generic.Dictionary<string, string> parameters)
		{
			throw new NotImplementedException ();
		}

		public SensorDefinition UpdateSensorDefinition (string name, string type, System.Collections.Generic.Dictionary<string, string> parameters)
		{
			throw new NotImplementedException ();
		}

		public void DeleteSensor (string sensorName)
		{
			throw new NotImplementedException ();
		}

		public void DeleteAll ()
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

