using System;

namespace FruitHAP.Controller.Rfx.InternalPacketData
{
	class RfxAckPacket
	{
		public byte SequenceNumber { get; set; }

		public override string ToString ()
		{
			return string.Format ("[RfxAckPacket: SequenceNumber={0}]", SequenceNumber);
		}
	}
}

