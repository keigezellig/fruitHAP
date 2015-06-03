using System;
using FruitHAP.Core.Action;
using FruitHAP.Core;
using Castle.Core.Logging;
using System.Collections.Generic;
using FluentValidation;
using FruitHap.ConfigurationActions.Messages;
using FruitHap.ConfigurationActions.Validators;
using System.Threading.Tasks;
using System.Linq;
using FruitHAP.Core.Sensor;
using FruitHAP.Common.Configuration;
using FruitHAP.Core.SensorConfiguration;

namespace FruitHap.ConfigurationActions.Messages
{
	public class GetSensorTypesRequest
	{
		public string SensorType { get; set;}	
	}

}

