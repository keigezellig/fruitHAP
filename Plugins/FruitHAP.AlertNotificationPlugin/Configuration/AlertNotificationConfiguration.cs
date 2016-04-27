using System;
using System.Collections.Generic;
using FruitHAP.Core.Plugin;

namespace FruitHAP.Plugins.AlertNotification.Configuration
{
	public class AlertNotificationConfiguration : BasePluginConfiguration
	{
		public string RoutingKey { get; set;}
		public List<NotificationConfiguration> Sensors { get; set;}
	}

}

