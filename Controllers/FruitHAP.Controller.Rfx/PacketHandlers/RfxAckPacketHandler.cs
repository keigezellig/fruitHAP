using System;
using Castle.Core.Logging;
using System.Linq;
using FruitHAP.Common.Helpers;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Controller;
using FruitHAP.Controller.Rfx.InternalPacketData;

namespace FruitHAP.Controller.Rfx.PacketHandlers
{
	public class RfxAckPacketHandler : IControllerPacketHandler
	{

		private readonly ILogger logger;
		private IEventAggregator aggregator;


		public RfxAckPacketHandler (ILogger logger, IEventAggregator aggregator)
		{
			this.logger = logger;
			this.aggregator = aggregator;
		}

		#region IControllerPacketHandler implementation

		public void Handle (byte[] data)
		{
			var sequenceNumber = data [3];
			logger.DebugFormat ("ACK received. Sequenceno = {0}",sequenceNumber);
			aggregator.GetEvent<RfxAckPacketEvent> ().Publish (new ControllerEventData<RfxAckPacket> () {
				Direction = Direction.FromController,
				Payload = new RfxAckPacket () { SequenceNumber = sequenceNumber }
			});

		}
		#endregion



	}
}

