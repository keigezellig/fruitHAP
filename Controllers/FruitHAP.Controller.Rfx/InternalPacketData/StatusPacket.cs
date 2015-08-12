﻿using System;

namespace FruitHAP.Controller.Rfx.InternalPacketData
{
	public class StatusPacket
	{
		public byte SequenceNumber { get; set; }
		public DeviceType DeviceType { get; set; }
		public ProtocolReceiverSensitivityFlags ReceiverSensitivity { get; set; }
		public string HardwareVersion { get; set; }
		public byte FirmwareVersion { get; set; }

		//SetMode response:
		//0D 01 00 04 03  53 E7 00 00 04 01 02 00 00
		//L  PT ST SQ CMD TT FV DV DV DV HW HW ? ?
		//(Same as GetStatus response, only CMD is different)
	

	}

	public enum DeviceType
	{
		Dt310 = 0x50,
		Dt315 = 0x51,
		Dt433ReceiverOnly = 0x52,
		Dt433Tranceiver = 0x53,
		Dt868 = 0x55,
		Dt868FSK = 0x56,
		Dt86830 = 0x57,
		Dt86830FSK = 0x58,
		Dt86835 = 0x59,
		Dt86835FSK = 0x5A,
		Dt86895 = 0x5B,
	}



}

