using System.Collections.Generic;

namespace FruitHap.StandardActions.IncomingRequest.RequestHandlers
{
	public class CommandObject
	{
		public string OperationName { get; set; }
		public Dictionary<string,string> Parameters {get; set;}
	}




}

