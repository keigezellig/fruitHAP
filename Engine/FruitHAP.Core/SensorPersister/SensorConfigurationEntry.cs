using System;

namespace FruitHAP.Core.SensorPersister
{
	public class SensorConfigurationEntry
	{
		public string Type { get; set; }
		public bool IsAggegrate {get; set;}
		public object Parameters { get; set; }
	}
}

