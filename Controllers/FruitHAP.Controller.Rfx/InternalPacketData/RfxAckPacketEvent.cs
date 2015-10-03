using System;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Controller;

namespace FruitHAP.Controller.Rfx.InternalPacketData
{
	class RfxAckPacketEvent : PubSubEvent<ControllerEventData<RfxAckPacket>>
	{
		
	}
}

