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


		public override string ToString ()
    	{
    		return string.Format ("[ConfigurationMessage: TimeStamp={0}, OperationName={1}, Parameters={2}, Data={3}, MessageType={4}]", TimeStamp, OperationName, Parameters, Data, MessageType);
    	}
    	
	}



    public enum ConfigurationMessageType
    {
        Request,
        Response,
        ErrorResponse
    }
}

