using System;

namespace FruitHAP.Sensor.Protocols.RawProtocol
{
	public class RawProtocolEventArgs :  EventArgs
	{
		public RawProtocolData Data { get; set; }
	}

}

