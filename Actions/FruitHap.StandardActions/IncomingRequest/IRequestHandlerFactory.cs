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
using System.Linq;

namespace FruitHap.StandardActions.IncomingRequest
{
	interface IRequestHandlerFactory
	{
		IRequestHandler GetRequestHandler (SensorMessage request);
	}




}

