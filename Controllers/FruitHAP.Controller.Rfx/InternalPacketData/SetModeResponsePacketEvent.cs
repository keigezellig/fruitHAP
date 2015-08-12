using System;
using FruitHAP.Core.Controller;
using Microsoft.Practices.Prism.PubSubEvents;

namespace FruitHAP.Controller.Rfx.InternalPacketData
{
	public class SetModeResponsePacketEvent : PubSubEvent<ControllerEventData<StatusPacket>>
	{
	}

}

