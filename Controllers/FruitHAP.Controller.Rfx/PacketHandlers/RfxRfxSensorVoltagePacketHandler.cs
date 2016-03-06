using System;
using FruitHAP.Core.Controller;
using Castle.Core.Logging;
using FruitHAP.Common.EventBus;
using FruitHAP.Core.Sensor.PacketData.RFXSensor;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Controller.Rfx.PacketHandlers
{
	public class RfxRfxSensorVoltagePacketHandler : IControllerPacketHandler
	{
		private ILogger logger;
		private IEventBus eventBus;

		public RfxRfxSensorVoltagePacketHandler (ILogger logger, IEventBus eventBus)
		{
			this.logger = logger;
			this.eventBus = eventBus;
		}

		#region IControllerPacketHandler implementation

		public void Handle (byte[] data)
		{
			var decodedData = GetData (data);
			eventBus.Publish(new ControllerEventData<RFXSensorVoltagePacket>() { Direction = Direction.FromController, Payload = decodedData});
		}
		#endregion

		private RFXSensorVoltagePacket GetData(byte[] rawData)
		{			
			var pdu = new RFXSensorVoltagePacket();
			pdu.SensorId = (byte)(rawData [4]);
			pdu.VoltageInDeciVolts = DecodeVoltage (rawData [5], rawData [6]);
			pdu.Level = (byte)(rawData [7] >> 4);

			logger.DebugFormat("Decoded packet: {0}", pdu);

			return pdu;
		}

		int DecodeVoltage (byte high, byte low)
		{
			int result = high * 256 + low;
			return result >> 3;
		}
	}
}

