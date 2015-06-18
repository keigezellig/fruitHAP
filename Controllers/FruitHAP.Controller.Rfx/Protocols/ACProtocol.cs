using System;
using Castle.Core.Logging;
using System.Linq;
using FruitHAP.Common.Helpers;
using System.Collections.Generic;
using FruitHAP.Core.Sensor;
using FruitHAP.Sensor.Protocols.ACProtocol;

namespace Controller.Rfx.ACProtocol
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

	public class ACProtocol : IACProtocol
	{
		private ILogger logger;
		private const byte Lighting2ProtocolIndicator = 0x11;
		private const byte ACPacketLength = 0x0B;
		private const byte ACPacketIndicator = 0x00;
		private const byte ClosingByte = 0x00;


		public ACProtocol(ILogger logger)
		{
			this.logger = logger;
		}

		public ACProtocolData Decode(byte[] rawData)
		{
			CheckLength(rawData);
			CheckProtocolIndicator(rawData);
			CheckPacketIndicator(rawData);

			return GetProtocolData(rawData);
		}

		public byte[] Encode(ACProtocolData protocolData)
		{
			List<byte> result = new List<byte> ();

			result.Insert (0, Lighting2ProtocolIndicator);
			result.Insert (1, ACPacketIndicator);
			result.Insert (2, byte.MaxValue);
			result.InsertRange (3, BitConverter.GetBytes (protocolData.DeviceId).Reverse());
			result.Insert (7, protocolData.UnitCode);
			result.Insert (8, (byte)protocolData.Command);
			result.Insert (9, byte.MaxValue);
			result.Insert (10, (byte)(protocolData.Level << 4));
			return result.ToArray ();
		}

		private ACProtocolData GetProtocolData(byte[] rawData)
		{
			byte[] deviceBytes = rawData.Skip(4).Take(4).Reverse().ToArray();
			logger.DebugFormat("{0}", deviceBytes.BytesAsString());

			var pdu = new ACProtocolData();
			pdu.DeviceId = BitConverter.ToUInt32(deviceBytes, 0);
			pdu.UnitCode = rawData[8];
			pdu.Command = (Command)rawData[9];
			pdu.Level = (byte)(rawData [11] >> 4);

			return pdu;
		}

		private void CheckPacketIndicator(byte[] rawData)
		{
			if (rawData[2] != 0x00)
			{
				throw new ProtocolException(string.Format("Incorrect packet indicator. This is not an AC packet. Packet indicator is 0x{0:X}",rawData[2]));
			}
		}

		private void CheckProtocolIndicator(byte[] rawData)
		{
			if (rawData[1] != 0x11)
			{
				throw new ProtocolException(string.Format("Incorrect protocol indicator. This is not an AC packet. Protocol indicator is 0x{0:X}", rawData[1]));
			}
		}

		private void CheckLength(byte[] rawData)
		{
			if (rawData [0] == ACPacketLength) 
			{
				throw new ProtocolException(string.Format("Incorrect length byte. This is not an AC packet. Actual length byte={0}", rawData[0]));
			}

			if (rawData.Count() != ACPacketLength + 1)
			{
				throw new ProtocolException(string.Format("Incorrect packet length. This is not an AC packet. Actual packet length={0}", rawData.Count() - 1));
			}
		}


	}
}

