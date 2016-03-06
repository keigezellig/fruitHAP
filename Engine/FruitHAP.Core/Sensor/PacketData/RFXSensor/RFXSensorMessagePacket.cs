using System;

namespace FruitHAP.Core.Sensor.PacketData.RFXSensor
{
	public class RFXSensorMessagePacket
	{
		public byte SensorId { get; set;}
		public MessageType MessageType { get; set; }

		public override string ToString ()
		{
			return string.Format ("[RFXSensorMessagePacket: SensorId={0}, MessageType={1}]", SensorId, MessageType);
		}
		
	}



	public enum MessageType
	{
		BatteryLow = 0x02
	}
}

