using System;
using Castle.Core.Logging;
using System.Linq;
using FruitHAP.Common.Helpers;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Controller;
using FruitHAP.Sensor.PacketData.AC;

namespace FruitHAP.Controller.Rfx.PacketHandlers
{
	public class RfxACPacketHandler : IControllerPacketHandler
	{

		private readonly ILogger logger;
		private IEventAggregator aggregator;


		public RfxACPacketHandler (ILogger logger, IEventAggregator aggregator)
		{
			this.logger = logger;
			this.aggregator = aggregator;
		}

		#region IControllerPacketHandler implementation

		public void Handle (byte[] data)
		{
			var decodedData = GetData (data);
			aggregator.GetEvent<ACPacketEvent>().Publish(new ControllerEventData<ACPacket>() { Direction = Direction.FromController, Payload = decodedData});
		}
		#endregion

		private ACPacket GetData(byte[] rawData)
		{
			byte[] deviceBytes = rawData.Skip(4).Take(4).Reverse().ToArray();
			logger.DebugFormat("{0}", deviceBytes.BytesAsString());

			var pdu = new ACPacket();
			pdu.DeviceId = BitConverter.ToUInt32(deviceBytes, 0);
			pdu.UnitCode = rawData[8];
			pdu.Command = (Command)rawData[9];
			pdu.Level = (byte)(rawData [11] >> 4);

			return pdu;
		}

	}
}

