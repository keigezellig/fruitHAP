using System;
using System.Collections.Generic;

namespace FruitHAP.Plugins.EventNotification.Configuration
{
	public class EventNotificationConfiguration
	{
		public string RoutingKey { get; set;}
		public List<String> Sensors { get; set;}
	}

}

