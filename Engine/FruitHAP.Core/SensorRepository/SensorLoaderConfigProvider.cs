using System;
using System.Collections.Generic;
using FruitHAP.Common.Configuration;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Core.SensorRepository
{
	public class SensorLoaderConfigProvider : ConfigProviderBase<List<SensorDefinition>>
	{
		public SensorLoaderConfigProvider (ILogger logger) : base (logger)
		{
		}
		
		protected override List<SensorDefinition> LoadFromFile (string fileName)
		{
			return JsonSerializerHelper.Deserialize<List<SensorDefinition>>(fileName);
		}

		protected override void SaveToFile (string fileName, List<SensorDefinition> config)
		{
			JsonSerializerHelper.Serialize (fileName, config);
		}

		protected override List<SensorDefinition> LoadDefaultConfig ()
		{
			return new List<SensorDefinition> ();
		}

	}
}

