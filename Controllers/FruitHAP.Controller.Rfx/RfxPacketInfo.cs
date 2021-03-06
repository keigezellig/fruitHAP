using System;

namespace FruitHAP.Controller.Rfx
{
	public class RfxPacketInfo
	{
		public RfxPacketType PacketType { get; set; }
		public byte LengthByte {get; set;}
		public byte PacketIndicator { get; set; }
		public byte SubPacketIndicator {get; set;}
		public ProtocolReceiverSensitivityFlags ProtocolReceiverSensitivityFlag { get; set; }
	}

	public enum RfxPacketType
	{
		Unknown,
		Interface,
		Ack,
		AC,
		RfxSensorTemperature,
		RfxSensorMessage,
		RfxSensorVoltage
	}


}
