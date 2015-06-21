using System;

namespace FruitHAP.Controller.Rfx
{
	[Flags]
	public enum RfxPacketType
	{
			Off = 0x00,
			X10 = 0x01,
			ARC = 0x02,
			AC = 0x04,
			HomeEasyEU = 0x08,
			MeianTech = 0x10,
			OregonScientific = 0x20,
			AtiRemote = 0x40,
			Visonic = 0x80,
			Mertik = 0x100,
			ADLightwaveRF = 0x200,
			HidekiUPM = 0x400,
			LaCrosse = 0x800,
			FS20 = 0x1000,
			ProGuard = 0x2000,
			BlindT0 = 0x4000,
			BlindT1T2T3T4 = 0x8000,
			AEBlyss = 0x10000,
			Rubicson = 0x20000,
			FineOffsetViking = 0x40000,
			Lighting4 = 0x80000,
			RSL2Revolt = 0x100000,
			ByronSX = 0x200000,
			RFU = 0x400000,
			Undecoded= 0x800000,
			
	}

	public class RfxPacketInfo
	{
		public RfxPacketType PacketType { get; set; }
		public byte LengthByte {get; set;}
		public byte PacketIndicator { get; set; }
		public byte SubPacketIndicator {get; set;}
	}

}

