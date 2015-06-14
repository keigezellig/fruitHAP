using System;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core
{
	public interface IReadOnlySwitch : ISensor
	{
		SwitchState GetState();
		event EventHandler<SwitchEventArgs> StateChanged;
	}

	public enum SwitchState
	{
		Undefined,On,Off
	}

	public class SwitchEventArgs : EventArgs
	{
		public SwitchState NewState { get; set;}
	}
}

