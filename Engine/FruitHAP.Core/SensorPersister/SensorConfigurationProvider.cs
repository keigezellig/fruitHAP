using System;
using System.Collections.Generic;
using FruitHAP.Common.Configuration;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Core.SensorPersister
{
	public class SensorConfigurationProvider : ConfigProviderBase<List<SensorConfigurationEntry>>
	{
		public SensorConfigurationProvider (ILogger logger) : base (logger)
		{
		}
		
		protected override List<SensorConfigurationEntry> LoadFromFile (string fileName)
		{
			return JsonSerializerHelper.Deserialize<List<SensorConfigurationEntry>>(fileName);
		}

		protected override void SaveToFile (string fileName, List<SensorConfigurationEntry> config)
		{
			JsonSerializerHelper.Serialize (fileName, config);
		}

		protected override List<SensorConfigurationEntry> LoadDefaultConfig ()
		{
			return new List<SensorConfigurationEntry> ();
		}

	}
}

