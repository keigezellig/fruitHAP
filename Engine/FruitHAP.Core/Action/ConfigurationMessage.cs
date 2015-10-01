using System;
using System.Collections.Generic;

namespace FruitHAP.Core
{
	public class ConfigurationMessage
	{
		public DateTime TimeStamp {get; set;}
		public string OperationName { get; set;}
		public Dictionary<string,string> Parameters {get; set;}
		public object Data {get;set;}
	}
}

