using FruitHap.StandardActions.Alarm;

namespace FruitHap.StandardActions.TemperatureAlarm
{

	public class TemperatureAlarmResponse
	{		
		public string NotificationText { get; set;}
		public NotificationPriority Priority {get; set;}
		public object OptionalData {get; set;}
	}

}
