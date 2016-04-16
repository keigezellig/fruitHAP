using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorValueTypes;


namespace FruitHAP.Core.Sensor.SensorTypes
{
	public interface ISwitch : IValueSensor
	{		
		OnOffValue State { get; }		
	}

	public enum Trigger
	{
		Both, On, Off
	}


}

