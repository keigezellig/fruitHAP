


using FruitHAP.Core.Action;

namespace FruitHap.StandardActions.IncomingRequest
{
	interface IRequestHandlerFactory
	{
		IRequestHandler GetRequestHandler (SensorMessage request);
	}

}

