using FruitHAP.Core.Sensor;

namespace FruitHap.StandardActions.Alarm
{

	public class AlarmResponse
	{		
		public string NotificationText { get; set;}
		public NotificationPriority Priority {get; set;}
		public OptionalDataContainer OptionalData {get; set;}
	}

}
