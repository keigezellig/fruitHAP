using System;
using System.Collections.Generic;

namespace FruitHap.StandardActions.EventTrigger.Configuration
{
	public class EventTriggerActionConfiguration
	{
		public string RoutingKey { get; set;}
		public List<string> Sensors { get; set;}
	}
}

