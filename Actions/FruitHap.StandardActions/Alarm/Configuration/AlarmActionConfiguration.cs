using System;
using System.Collections.Generic;

namespace FruitHap.StandardActions.Alarm.Configuration
{
	public class AlarmActionConfiguration
	{
		public string RoutingKey { get; set;}
		public List<NotificationConfiguration> Sensors { get; set;}
	}

}

