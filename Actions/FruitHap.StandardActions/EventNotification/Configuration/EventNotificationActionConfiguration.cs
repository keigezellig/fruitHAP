using System;
using System.Collections.Generic;

namespace FruitHap.StandardActions.EventNotification.Configuration
{
	public class EventNotificationActionConfiguration
	{
		public string RoutingKey { get; set;}
		public List<String> Sensors { get; set;}
	}

}

