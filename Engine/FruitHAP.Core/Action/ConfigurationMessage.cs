using System;
using System.Collections.Generic;

namespace FruitHAP.Core.Action
{
	/// <summary>
    /// 
    /// </summary>
    public class ConfigurationMessage
	{
		public DateTime TimeStamp {get; set;}
		public string OperationName { get; set;}
		public Dictionary<string,string> Parameters {get; set;}
		public object Data {get;set;}
        public ConfigurationMessageType MessageType { get; set; }
	}

    public enum ConfigurationMessageType
    {
        Request,
        Response,
        ErrorResponse
    }
}

