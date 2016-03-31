using System;

namespace FruitHAP.Sensor.PacketData.AC
{
	public class ACPacket
	{
        //public DateTime Timestamp {get;set;}
        public uint DeviceId { get; set; }
		public byte UnitCode { get; set; }
		public ACCommand Command { get; set; }
		public byte Level { get; set; }

		public override string ToString()
		{
			return string.Format("Device id: {0:X} ({0})\nUnit code: {1}\nCommand:{2}\nLevel:{3}", DeviceId, UnitCode, Command,
				Level);
		}
	}

	public enum ACCommand
	{
		Off = 0,
		On = 1,
		SetLevel = 2,
		GroupOff = 3,
		GroupOn = 4,
		SetGroupLevel = 5
	}
}

