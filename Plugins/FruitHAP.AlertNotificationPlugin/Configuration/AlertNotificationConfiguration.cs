using System;
using System.Collections.Generic;

namespace FruitHAP.Plugins.AlertNotification.Configuration
{
	public class AlertNotificationConfiguration
	{
		public string RoutingKey { get; set;}
		public List<NotificationConfiguration> Sensors { get; set;}
	}

}

