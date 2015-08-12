using System;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Controller;

namespace FruitHAP.Sensor.PacketData.AC
{
	public class ACPacketEvent : PubSubEvent<ControllerEventData<ACPacket>>
	{
	}
}

