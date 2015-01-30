using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
     * byte 9: Command
     *         S
     */

    public class KakuProtocol : IKakuProtocol
    {
        public KakuProtocolData Decode(byte[] rawData)
        {
            throw new NotImplementedException();
        }

        public byte[] Encode(KakuProtocolData protocolData)
        {
            throw new NotImplementedException();
        }
    }
}
