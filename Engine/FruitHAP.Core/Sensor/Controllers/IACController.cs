using System;
using FruitHAP.Core.Sensor;
using FruitHAP.Sensor.Protocols.ACProtocol;

namespace FruitHAP.Core.Sensor.Controllers
{
	public interface IACController : ISensorController
	{
		event EventHandler<ACProtocolEventArgs> ACDataReceived;
		void SendACData(ACProtocolData data);

	}
}

