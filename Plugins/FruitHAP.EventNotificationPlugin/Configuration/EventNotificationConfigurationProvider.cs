using System;
using FruitHAP.Common.Configuration;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Plugins.EventNotification.Configuration
{
	public class EventNotificationConfigurationProvider : ConfigProviderBase<EventNotificationConfiguration>
	{
		public EventNotificationConfigurationProvider (ILogger logger) : base (logger)
		{
		}

		protected override EventNotificationConfiguration LoadFromFile(string fileName)
		{
			return JsonSerializerHelper.Deserialize<EventNotificationConfiguration>(fileName);
		}

		protected override void SaveToFile(string fileName, EventNotificationConfiguration config)
		{
			JsonSerializerHelper.Serialize(fileName,config);
		}

		protected override EventNotificationConfiguration LoadDefaultConfig()
		{
			return new EventNotificationConfiguration() { RoutingKey = "events"};
		}
		
	}
}

