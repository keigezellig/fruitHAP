using System;
using FruitHAP.Common.Configuration;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;

namespace FruitHap.StandardActions.TemperatureAlarm.Configuration
{
	public class TemperatureAlarmActionConfigurationProvider : ConfigProviderBase<TemperatureAlarmActionConfiguration>
	{
		public TemperatureAlarmActionConfigurationProvider (ILogger logger) : base (logger)
		{
		}

		protected override TemperatureAlarmActionConfiguration LoadFromFile(string fileName)
		{
			return JsonSerializerHelper.Deserialize<TemperatureAlarmActionConfiguration>(fileName);
		}

		protected override void SaveToFile(string fileName, TemperatureAlarmActionConfiguration config)
		{
			JsonSerializerHelper.Serialize(fileName,config);
		}

		protected override TemperatureAlarmActionConfiguration LoadDefaultConfig()
		{
			return new TemperatureAlarmActionConfiguration () {
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

