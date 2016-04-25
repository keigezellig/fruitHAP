using System;
using System.Collections.Generic;
using FruitHAP.Core.Plugin;

namespace FruitHAP.Plugins.Thermostat.Configuration
{
	public class ThermostatConfiguration : BasePluginConfiguration
	{
		public string RoutingKey { get; set;}
		public string NotificationTextAbove {get; set;}
		public string NotificationTextBelow {get; set;}
		public string TemperatureSensor {get; set;}
		public string SwitchAbove {get; set;}
		public string SwitchBelow {get; set;}
		public int ThresholdHot { get; set;}
		public int ThresholdCold { get; set;} 
	}

}

