using System;
using Nancy;
using FruitHAP.Core.SensorPersister;

namespace FruitHap.RestInterface
{
	public class ConfigurationModule : NancyModule
	{
		ISensorPersister persister;

		public ConfigurationModule (ISensorPersister persister)
		{
			this.persister = persister;
			bool isNull = persister == null;
			Get["/"] = _ => "Hello World! " + isNull.ToString();
			
		}
	}
}

