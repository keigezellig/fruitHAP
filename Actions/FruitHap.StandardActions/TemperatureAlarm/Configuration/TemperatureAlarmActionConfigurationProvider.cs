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
				Switch = "BlueLight",
				NotificationText = "Too hot in here!",
				TemperatureSensor = "MyTempSensor",
				Threshold = 25
			};
	
		}
	}
}

