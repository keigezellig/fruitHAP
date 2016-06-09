using System;
using FruitHAP.Core.Controller;
using Castle.Core.Logging;
using FruitHAP.Common.EventBus;
using FruitHAP.Core.Sensor.PacketData.RFXSensor;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Controller.Rfx.PacketHandlers
{
    public class RfxRfxMeterPacketHandler : IControllerPacketHandler
	{
		private ILogger logger;
		private IEventBus eventBus;

        public RfxRfxMeterPacketHandler (ILogger logger, IEventBus eventBus)
		{
			this.logger = logger;
			this.eventBus = eventBus;
		}

		#region IControllerPacketHandler implementation

		public void Handle (byte[] data)
		{
			var decodedData = GetData (data);
			eventBus.Publish(new ControllerEventData<RFXMeterPacket>() { Direction = Direction.FromController, Payload = decodedData});
		}
		#endregion

        private RFXMeterPacket GetData(byte[] rawData)
		{			
            var pdu = new RFXMeterPacket();
			pdu.SensorId = (byte)(rawData [4]);

            pdu.Value = (uint)(rawData[7] << 16) + (uint)(rawData[8] << 8) + (uint)rawData[9];
			pdu.Level = (byte)(rawData [10] >> 4);

			logger.DebugFormat("Decoded packet: {0}", pdu);

			return pdu;
		}
	}
}

