using System;
using FruitHAP.Common.Configuration;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;

namespace FruitHap.StandardActions.EventNotification.Configuration
{
	public class EventNotificationActionConfigurationProvider : ConfigProviderBase<EventNotificationActionConfiguration>
	{
		public EventNotificationActionConfigurationProvider (ILogger logger) : base (logger)
		{
		}

		protected override EventNotificationActionConfiguration LoadFromFile(string fileName)
		{
			return JsonSerializerHelper.Deserialize<EventNotificationActionConfiguration>(fileName);
		}

		protected override void SaveToFile(string fileName, EventNotificationActionConfiguration config)
		{
			JsonSerializerHelper.Serialize(fileName,config);
		}

		protected override EventNotificationActionConfiguration LoadDefaultConfig()
		{
			return new EventNotificationActionConfiguration() { RoutingKey = "alerts", Sensors = new System.Collections.Generic.List<string>()};
		}
		
	}
}

