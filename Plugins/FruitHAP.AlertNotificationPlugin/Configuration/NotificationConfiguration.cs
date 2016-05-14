using System;
using System.Collections.Generic;

namespace FruitHAP.Plugins.AlertNotification.Configuration
{

	public class NotificationConfiguration
	{
		public string SensorName { get; set; }
		public string NotificationText { get; set;}
		public NotificationPriority Priority {get; set;}
	}

}
