using System;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.Sensor.SensorTypes
{
	public interface IButtonWithCameraSensor : IAggregatedSensor
	{
		event EventHandler<CameraImageEventArgs> DataChanged;
	}

	public class CameraImageEventArgs : EventArgs
	{
		public byte[] CameraImage { get; set;}
	}
}

