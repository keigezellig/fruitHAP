using System;
using Castle.Core.Logging;
using System.Linq;
using FruitHAP.Common.Helpers;
using System.Collections.Generic;
using FruitHAP.Core.Sensor;
using FruitHAP.Sensor.PacketData.AC;

namespace Controller.Rfx.PacketHandlers
{
	/* The protocol used in the KlikAanKlikUit is the AC subpacket in the LIGHTING2 protocol which is a X10 derivative.
     * 
     * It consists of 12 bytes (unsigned) with the following structure:
     * 
     * eg. 0x0B 0x11 0x00 0x02 0x00 0xE4 0x18 0x22 0x01 0x04 0x07 0x00 
     * 
     * (from left to right)
     * byte 0: Packet Length byte (always 0x0B (11) and NOT including this byte )
     * byte 1: Protocol indicator for LIGHTING2 (0x11)
     * byte 2: Packet indicator (0x00=AC (KaKu) )
     * byte 3: Not used in protocol
     * byte 4+5+6+7: Device ID (0xE41822)
     * byte 8: Unit code (0x01)
     * byte 9: Command (0x04)
     * byte 10: ?
     * byte 11: Level (MSN so >> 4) (0x00)
     *         
     */

	public class RfxACProtocol 
	{
		private ILogger logger;
		private const byte Lighting2ProtocolIndicator = 0x11;
		private const byte ACPacketLength = 0x0B;
		private const byte ACPacketIndicator = 0x00;
		private const byte ClosingByte = 0x00;


		public RfxACProtocol(ILogger logger)
		{
			this.logger = logger;
		}
			
		public byte[] Encode(ACPacket protocolData)
		{
			List<byte> result = new List<byte> ();
			result.Insert (0, ACPacketLength);
			result.Insert (1, Lighting2ProtocolIndicator);
			result.Insert (2, ACPacketIndicator);
			result.Insert (3, byte.MaxValue);
			result.InsertRange (4, BitConverter.GetBytes (protocolData.DeviceId).Reverse());
			result.Insert (8, protocolData.UnitCode);
			result.Insert (9, (byte)protocolData.Command);
			result.Insert (10, byte.MaxValue);
			result.Insert (11, (byte)(protocolData.Level << 4));
			return result.ToArray ();
		}
	}
}

