using System;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.SensorEventPublisher;

namespace FruitHAP.Sensor.Aggregated.Events
{
	public class CameraEvent : PubSubEvent<EventData>
	{
	}
}

