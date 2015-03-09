using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;

namespace FruitHAP.SensorModule.Kaku.Protocol
{
    /* The protocol used in the KlikAanKlikUit is the AC subpacket in the LIGHTING2 protocol which is a X10 derivative.
     * 
     * It consists of 11 bytes (unsigned) with the following structure:
     * 
     * eg. 0x0B 0x11 0x00 0x02 0x00 0xE4 0x18 0x22 0x01 0x04 0x07 0x00 
     * 
     * (from left to right)
     * byte 0+1: Protocol indicator for LIGHTING2 (0x0B 0x11)
     * byte 2: Packet indicator (0x00=AC (KaKu) )
     * byte 3: Sequence indicator (is not used in protcol itself, but used in the physical communication with the particular RFX receiver)
     * byte 4+5+6+7: Device ID (0xE41822)
     * byte 8: Unit code (0x01)
     * byte 9: Command (0x04)
     * byte 10: Level
     * byte 11: Closing byte (always 0x00)
     *         
     */

    public class KakuProtocol : IKakuProtocol
    {
        private ILogger logger;
		private byte[] ProtocolHeader = new byte[]{0x0B, 0x11};
		private byte PacketIndicator = 0x00;
		private byte ClosingByte = 0x00;


        public KakuProtocol(ILogger logger)
        {
            this.logger = logger;
        }

        public KakuProtocolData Decode(byte[] rawData)
        {
            CheckLength(rawData);
            CheckProtocolIndicator(rawData);
            CheckPacketIndicator(rawData);
            
            return GetProtocolData(rawData);
        }

        private KakuProtocolData GetProtocolData(byte[] rawData)
        {
            byte[] deviceBytes = rawData.Skip(4).Take(4).Reverse().ToArray();
            logger.DebugFormat("{0}", deviceBytes.BytesAsString());

            var pdu = new KakuProtocolData();
            pdu.DeviceId = BitConverter.ToUInt32(deviceBytes, 0);
            pdu.UnitCode = rawData[8];
            pdu.Command = (Command)rawData[9];
            pdu.Level = rawData[10];

            return pdu;
        }

        private void CheckPacketIndicator(byte[] rawData)
        {
            if (rawData[2] != 0x00)
            {
                throw new ProtocolException(string.Format("Incorrect packet indicator. This is not a KaKu packet. Packet indicator is 0x{0:X}",rawData[2]));
            }
        }

        private void CheckProtocolIndicator(byte[] rawData)
        {
            if (rawData[0] != 0x0B || rawData[1] != 0x11)
            {
                throw new ProtocolException(string.Format("Incorrect protocol indicator. This is not a KaKu packet. Protocol indicator is 0x{0:X} 0x{1:X}", rawData[0], rawData[1]));
            }
        }

        private void CheckLength(byte[] rawData)
        {
            if (rawData.Count() != 12)
            {
                throw new ProtocolException(string.Format("Incorrect packet length. This is not a KaKu packet. Length={0}", rawData.Count()));
            }
        }

        public byte[] Encode(KakuProtocolData protocolData)
        {
			List<byte> result = new List<byte> ();
			result.InsertRange (0, ProtocolHeader);
			result.Insert (2, PacketIndicator);
			result.Insert (3, byte.MaxValue);
			result.InsertRange (4, BitConverter.GetBytes (protocolData.DeviceId).Reverse());
			result.Insert (8, protocolData.UnitCode);
			result.Insert (9, (byte)protocolData.Command);
			result.Insert (10, protocolData.Level);
			result.Insert (11, ClosingByte);
			return result.ToArray ();
        }
    }
}
