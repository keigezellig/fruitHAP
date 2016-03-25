using FruitHAP.Core.Sensor;

namespace FruitHAP.Plugins.Thermostat
{

	public class ThermostatResponse
	{		
		public string NotificationText { get; set;}
		public NotificationPriority Priority {get; set;}
		public OptionalDataContainer OptionalData {get; set;}
	}

}
