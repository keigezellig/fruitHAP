using System;
using FruitHAP.Core.Controller;
using Castle.Core.Logging;
using FruitHAP.Common.EventBus;
using FruitHAP.Core.Sensor.PacketData.RFXSensor;

namespace FruitHAP.Controller.Rfx.PacketHandlers
{
	public class RfxRfxSensorMessagePacketHandler : IControllerPacketHandler
	{
		private ILogger logger;
		private IEventBus eventBus;

		public RfxRfxSensorMessagePacketHandler (ILogger logger, IEventBus eventBus)
		{
			this.logger = logger;
			this.eventBus = eventBus;
		}

		#region IControllerPacketHandler implementation
		public void Handle (byte[] data)
		{
			var decodedData = GetData (data);
			eventBus.Publish(new ControllerEventData<RFXSensorMessagePacket>() { Direction = Direction.FromController, Payload = decodedData});
		}
		#endregion

		private RFXSensorMessagePacket GetData(byte[] rawData)
		{			
			var pdu = new RFXSensorMessagePacket();
			pdu.SensorId = (byte)(rawData [4]);
			pdu.MessageType = (MessageType)(rawData [6] >> 3);

			logger.DebugFormat("Decoded packet: {0}", pdu);

			return pdu;
		}

	}
}

