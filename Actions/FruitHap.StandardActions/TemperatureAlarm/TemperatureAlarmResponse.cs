using FruitHap.StandardActions.Alarm;
using FruitHAP.Core.Sensor;

namespace FruitHap.StandardActions.TemperatureAlarm
{

	public class TemperatureAlarmResponse
	{		
		public string NotificationText { get; set;}
		public NotificationPriority Priority {get; set;}
		public OptionalDataContainer OptionalData {get; set;}
	}

}
