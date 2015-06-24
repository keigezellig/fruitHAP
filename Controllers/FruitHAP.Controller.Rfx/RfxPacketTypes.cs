using System;

namespace FruitHAP.Controller.Rfx
{
	public enum RfxPacketType
	{
		Unknown,
		AC

	}

	public class RfxPacketInfo
	{
		public RfxPacketType PacketType { get; set; }
		public byte LengthByte {get; set;}
		public byte PacketIndicator { get; set; }
		public byte SubPacketIndicator {get; set;}
	}

}

