using System;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;

namespace FruitHAP.Core.Sensor.SensorTypes
{
	public interface ISwitchWithCameraSensor: IAggregatedSensor
	{
		event EventHandler<CameraImageEventArgs> DataChanged;
	}
}

