using System;
using FruitHAP.Core.Controller;
using Castle.Core.Logging;
using FruitHAP.Common.EventBus;
using FruitHAP.Core.Sensor.PacketData.RFXSensor;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Controller.Rfx.PacketHandlers
{
	public class RfxRfxSensorTemperaturePacketHandler : IControllerPacketHandler
	{
		private ILogger logger;
		private IEventBus eventBus;

		public RfxRfxSensorTemperaturePacketHandler (ILogger logger, IEventBus eventBus)
		{
			this.logger = logger;
			this.eventBus = eventBus;
		}

		#region IControllerPacketHandler implementation

		public void Handle (byte[] data)
		{
			var decodedData = GetData (data);
			eventBus.Publish(new ControllerEventData<RFXSensorTemperaturePacket>() { Direction = Direction.FromController, Payload = decodedData});
		}
		#endregion

		private RFXSensorTemperaturePacket GetData(byte[] rawData)
		{			
			var pdu = new RFXSensorTemperaturePacket();
			pdu.SensorId = (byte)(rawData [4]);
			pdu.TemperatureInCentiCelsius = DecodeTemperature (rawData [5], rawData [6]);
			pdu.Level = (byte)(rawData [7] >> 4);

			logger.DebugFormat("Decoded packet: {0}", pdu);

			return pdu;
		}

		int DecodeTemperature (byte high, byte low)
		{
			bool isNegative = high.IsBitSet (7);
			byte tmpHigh = high.ClearBit (7);
			int temperature = (tmpHigh << 8) + low;
			return isNegative ? -temperature : temperature;
		}
	}
}

