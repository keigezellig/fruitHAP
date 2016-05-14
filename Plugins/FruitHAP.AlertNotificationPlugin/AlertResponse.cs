using FruitHAP.Core.Sensor;

namespace FruitHAP.Plugins.AlertNotification
{

	public class AlertResponse
	{		
		public string NotificationText { get; set;}
		public NotificationPriority Priority {get; set;}
		public OptionalDataContainer OptionalData {get; set;}
	}

}
