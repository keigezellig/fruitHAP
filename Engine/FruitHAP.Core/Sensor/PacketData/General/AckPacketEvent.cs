using System;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Controller;

namespace FruitHAP.Sensor.PacketData.General
{
	public class AckPacketEvent : PubSubEvent<ControllerEventData<AckPacket>>
	{
	}
}

