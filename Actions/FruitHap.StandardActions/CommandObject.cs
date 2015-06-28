using System;
using FruitHAP.Core.Action;
using FruitHAP.Core.SensorRepository;
using Castle.Core.Logging;
using FruitHAP.Core.MQ;
using FruitHap.StandardActions.Messages.Outbound;
using System.Threading.Tasks;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Common.Helpers;
using System.Collections.Generic;

namespace FruitHap.StandardActions
{
	public class CommandObject
	{
		public string OperationName { get; set; }
		public Dictionary<string,string> Parameters {get; set;}
	}




}

