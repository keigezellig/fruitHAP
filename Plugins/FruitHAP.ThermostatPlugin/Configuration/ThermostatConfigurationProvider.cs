using System;
using FruitHAP.Common.Configuration;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Plugins.Thermostat.Configuration
{
	public class ThermostatConfigurationProvider : ConfigProviderBase<ThermostatConfiguration>
	{
		public ThermostatConfigurationProvider (ILogger logger) : base (logger)
		{
		}

		protected override ThermostatConfiguration LoadFromFile(string fileName)
		{
			return JsonSerializerHelper.Deserialize<ThermostatConfiguration>(fileName);
		}

		protected override void SaveToFile(string fileName, ThermostatConfiguration config)
		{
			JsonSerializerHelper.Serialize(fileName,config);
		}

		protected override ThermostatConfiguration LoadDefaultConfig()
		{
			return new ThermostatConfiguration () {
				RoutingKey = "alerts",
				SwitchAbove = "RedLight",
				SwitchBelow = "BlueLight",
				NotificationTextAbove = "Too hot in here!",
				NotificationTextBelow = "It's fffreeezing",
				TemperatureSensor = "MyTempSensor",
				ThresholdHot = 25,
				ThresholdCold = 22
					
			};
	
		}
	}
}

