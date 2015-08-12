using System;
using FruitHAP.Common.Configuration;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;

namespace FruitHap.StandardActions.EventTrigger.Configuration
{
	public class EventTriggerActionConfigurationProvider : ConfigProviderBase<EventTriggerActionConfiguration>
	{
		public EventTriggerActionConfigurationProvider (ILogger logger) : base (logger)
		{
		}

		protected override EventTriggerActionConfiguration LoadFromFile(string fileName)
		{
			return JsonSerializerHelper.Deserialize<EventTriggerActionConfiguration>(fileName);
		}

		protected override void SaveToFile(string fileName, EventTriggerActionConfiguration config)
		{
			JsonSerializerHelper.Serialize(fileName,config);
		}

		protected override EventTriggerActionConfiguration LoadDefaultConfig()
		{
			return new EventTriggerActionConfiguration() { RoutingKey = "alerts", Sensors = new System.Collections.Generic.List<string>()};
		}
		
	}
}

