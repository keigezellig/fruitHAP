using FruitHap.StandardActions.Messages;

namespace FruitHap.StandardActions.IncomingRequest
{
	interface IRequestHandlerFactory
	{
		IRequestHandler GetRequestHandler (SensorMessage request);
	}




}

