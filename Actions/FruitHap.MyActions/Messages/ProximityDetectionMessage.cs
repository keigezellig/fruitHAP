using System;
using EasyNetQ;

namespace FruitHap.MyActions
{
	[Queue("FruitHAPQueue", ExchangeName = "FruitHAPExchange")]
	public class ProximityDetectionMessage
	{
		public DateTime Timestamp { get; set; }
		public string DetectorName { get; set; }

	}
}

