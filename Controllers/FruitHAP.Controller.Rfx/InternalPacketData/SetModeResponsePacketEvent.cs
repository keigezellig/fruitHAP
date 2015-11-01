using System;
using FruitHAP.Core.Controller;
using Microsoft.Practices.Prism.PubSubEvents;

namespace FruitHAP.Controller.Rfx.InternalPacketData
{
	class SetModeResponsePacketEvent : PubSubEvent<ControllerEventData<StatusPacket>>
	{
	}

}

