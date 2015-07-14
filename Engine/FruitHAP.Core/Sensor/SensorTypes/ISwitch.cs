using System;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.Sensor.SensorTypes
{
	public interface ISwitch : IValueSensor
	{
		SwitchState GetState();
		event EventHandler<SwitchEventArgs> StateChanged;


	}

	public enum SwitchState
	{
		Undefined,On,Off
	}

	public enum Trigger
	{
		Both, On, Off
	}


	public class SwitchEventArgs : EventArgs
	{
		public SwitchState NewState { get; set;}
	}
}

