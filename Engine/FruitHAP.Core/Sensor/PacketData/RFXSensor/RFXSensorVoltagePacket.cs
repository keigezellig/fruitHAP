using System;

namespace FruitHAP.Core.Sensor.PacketData.RFXSensor
{
	public class RFXSensorVoltagePacket
	{
		public byte SensorId { get; set;}
		public int VoltageInDeciVolts { get; set; }
		public byte Level { get; set; }

		public override string ToString ()
		{
			return string.Format ("[RFXSensorVoltagePacket: SensorId={0}, VoltageInDeciVolts={1}, Level={2}]", SensorId, VoltageInDeciVolts, Level);
		}
		
	}
}

