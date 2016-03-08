using System;
using System.Collections.Generic;

namespace FruitHap.StandardActions.TemperatureAlarm.Configuration
{
	public class TemperatureAlarmActionConfiguration
	{
		public string RoutingKey { get; set;}
		public string NotificationText {get; set;}
		public string TemperatureSensor {get; set;}
		public string Switch {get; set;}
		public int Threshold { get; set;} 


	}

}

