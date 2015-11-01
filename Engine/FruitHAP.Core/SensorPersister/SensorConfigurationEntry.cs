using System;

namespace FruitHAP.Core.SensorPersister
{
	public class SensorConfigurationEntry
	{
		public string Type { get; set; }
		public bool IsAggegrate {get; set;}
		public object Parameters { get; set; }

		public override string ToString ()
		{
			return string.Format ("[SensorConfigurationEntry: Type={0}, IsAggegrate={1}, Parameters={2}]", Type, IsAggegrate, Parameters);
		}
		
	}
}

