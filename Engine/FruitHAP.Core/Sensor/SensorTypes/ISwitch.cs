using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorValueTypes;


namespace FruitHAP.Core.Sensor.SensorTypes
{
	public interface ISwitch : IValueSensor
	{
		bool IsReadOnly { get; set; }
		OnOffValue State { get; }
		void TurnOn();
		void TurnOff();
	}

	public enum Trigger
	{
		Both, On, Off
	}


}

