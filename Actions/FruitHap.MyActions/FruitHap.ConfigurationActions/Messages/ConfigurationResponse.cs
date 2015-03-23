using System;

namespace FruitHap.ConfigurationActions
{
	public class ConfigurationResponse
	{
		public Result Result {get; set;}
		public FaultReason FaultReason {get; set;}
		public string FaultMessage {get;set;}
		public object ResultData { get; set; }
	}

	public enum Result
	{
		Ok, NotOk
	}

	public enum FaultReason
	{
		InvalidCommand, ParameterError, InternalError
	}
}

