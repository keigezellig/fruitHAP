using System;
using EasyNetQ;

namespace FruitHap.StandardActions.Messages.Outbound
{	
	[Queue("FruitHAPQueue", ExchangeName = "FruitHAPExchange")]
	public class SensorMessage
	{
		public DateTime TimeStamp {get; set;}
		public string SensorName { get; set;}
		public string SensorType { get; set;}
		public object Data {get; set;}
		public string DataType {get; set;}

		public override string ToString ()
		{
			return string.Format ("[SensorMessage: TimeStamp={0}, SensorName={1}, SensorType={2}, Data={3}, DataType={4}]", TimeStamp, SensorName, SensorType, Data, DataType);
		}
		
	}


	public enum DataType
	{
		Undefined,
		Command,
		GetValue,
		ErrorMessage,
		Event,
		Measurement
	}
}

