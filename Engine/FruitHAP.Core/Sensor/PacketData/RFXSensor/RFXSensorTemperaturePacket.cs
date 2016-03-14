using System;

namespace FruitHAP.Core.Sensor.PacketData.RFXSensor
{
	public class RFXSensorTemperaturePacket
	{
		public byte SensorId { get; set;}
		public int TemperatureInCentiCelsius { get; set; }
		public byte Level { get; set; }

		public override string ToString ()
		{
			return string.Format ("[RFXSensorTemperaturePacket: SensorId={0}, TemperatureInCentiCelsius={1}, Level={2}]", SensorId, TemperatureInCentiCelsius, Level);
		}
		
	}
}

