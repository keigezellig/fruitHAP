using System;
using FruitHAP.Common.Configuration;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Plugins.AlertNotification.Configuration
{
	public class AlertNotificationConfigurationProvider : ConfigProviderBase<AlertNotificationConfiguration>
	{
		public AlertNotificationConfigurationProvider (ILogger logger) : base (logger)
		{
		}

		protected override AlertNotificationConfiguration LoadFromFile(string fileName)
		{
			return JsonSerializerHelper.Deserialize<AlertNotificationConfiguration>(fileName);
		}

		protected override void SaveToFile(string fileName, AlertNotificationConfiguration config)
		{
			JsonSerializerHelper.Serialize(fileName,config);
		}

		protected override AlertNotificationConfiguration LoadDefaultConfig()
		{
			return new AlertNotificationConfiguration() { RoutingKey = "alerts", Sensors = new System.Collections.Generic.List<NotificationConfiguration>()};
		}
		
	}
}

