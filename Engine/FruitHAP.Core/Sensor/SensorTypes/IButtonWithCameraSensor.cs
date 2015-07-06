using System;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.SensorTypes
{
	public interface IButtonWithCameraSensor : IAggregatedSensor
	{
		event EventHandler<ButtonWithCameraSensorEventArgs> DataChanged;
	}

	public class ButtonWithCameraSensorEventArgs : EventArgs
	{
		public byte[] CameraImage { get; set;}
	}
}

