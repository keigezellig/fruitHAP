using System;
using FruitHAP.Common.Configuration;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;

namespace FruitHap.StandardActions.Alarm.Configuration
{
	public class AlarmActionConfigurationProvider : ConfigProviderBase<AlarmActionConfiguration>
	{
		public AlarmActionConfigurationProvider (ILogger logger) : base (logger)
		{
		}

		protected override AlarmActionConfiguration LoadFromFile(string fileName)
		{
			return JsonSerializerHelper.Deserialize<AlarmActionConfiguration>(fileName);
		}

		protected override void SaveToFile(string fileName, AlarmActionConfiguration config)
		{
			JsonSerializerHelper.Serialize(fileName,config);
		}

		protected override AlarmActionConfiguration LoadDefaultConfig()
		{
			return new AlarmActionConfiguration() { RoutingKey = "alerts", Sensors = new System.Collections.Generic.List<NotificationConfiguration>()};
		}
		
	}
}

