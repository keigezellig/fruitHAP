using System;
using EasyNetQ;

namespace FruitHAP.Core.Action
{	
	[Queue("FruitHAPQueue", ExchangeName = "FruitHAPExchange")]
	public class SensorMessage
	{
		public DateTime TimeStamp {get; set;}
		public string SensorName { get; set;}
		public object Data {get; set;}
		public string EventType {get; set;}

		public override string ToString ()
		{
			return string.Format ("[SensorMessage: TimeStamp={0}, SensorName={1}, Data={2}, EventType={3}]", TimeStamp, SensorName, Data, EventType);
		}
		
	}


}

