using System;
using FruitHAP.Core.Sensor;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.SensorEventPublisher;

namespace FruitHAP.Core.Sensor.SensorTypes
{
	public interface ISwitch : IValueSensor
	{
		SwitchState GetState();
	}

	public enum SwitchState
	{
		Undefined,On,Off
	}

	public enum Trigger
	{
		Both, On, Off
	}


}

