using System;
using System.Collections.Generic;

namespace FruitHap.StandardActions.EventTrigger.Configuration
{
	public class EventTriggerActionConfiguration
	{
		public string RoutingKey { get; set;}
		public List<EventNotificationConfiguration> Sensors { get; set;}
	}

	public class EventNotificationConfiguration
	{
		public string SensorName { get; set; }
		public string NotificationText { get; set;}
		public NotificationPriority Priority {get; set;}
	}

	public enum NotificationPriority
	{
		Low, Medium, High
	}


}

