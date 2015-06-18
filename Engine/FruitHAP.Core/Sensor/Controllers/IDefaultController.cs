using System;
using FruitHAP.Sensor.Protocols.RawProtocol;

namespace FruitHAP.Core.Sensor.Controllers
{
	public interface IDefaultController : ISensorController
	{
		event EventHandler<RawProtocolEventArgs> ACDataReceived;
		void SendData(RawProtocolData data);

	}
}

