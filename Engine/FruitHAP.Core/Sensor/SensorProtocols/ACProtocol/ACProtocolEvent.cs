using System;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Sensor.Controllers;

namespace FruitHAP.Sensor.Protocols.ACProtocol
{
	public class ACProtocolEvent : PubSubEvent<ControllerEventData<ACProtocolData>>
	{
	}
}

