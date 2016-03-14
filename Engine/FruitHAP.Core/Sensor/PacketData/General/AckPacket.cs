using System;

namespace FruitHAP.Sensor.PacketData.General
{
	public class AckPacket
	{
		public bool IsAcknowledged { get; set; }

		public override string ToString ()
		{
			return string.Format ("[AckPacket: IsAcknowledged={0}]", IsAcknowledged);
		}
	}
}

