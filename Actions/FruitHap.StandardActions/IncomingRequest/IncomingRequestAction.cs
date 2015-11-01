using FruitHAP.Core.Action;
using FruitHAP.Core.SensorRepository;
using Castle.Core.Logging;
using FruitHAP.Core.MQ;

namespace FruitHap.StandardActions.IncomingRequest
{
    public class IncomingRequestAction : RpcAction<SensorMessage,SensorMessage>
	{
		private readonly ISensorRepository sensorRepository;
		private readonly IRequestHandlerFactory requestHandlerFactory;

		public IncomingRequestAction(ISensorRepository sensorRepository, ILogger logger, IMessageQueueProvider publisher) : base(logger,publisher)
		{
			this.sensorRepository = sensorRepository;			
			this.requestHandlerFactory = new RequestHandlerFactory(logger,sensorRepository);
		}

        protected override SensorMessage ProcessRequest(SensorMessage request)
        {
            IRequestHandler handler = requestHandlerFactory.GetRequestHandler(request);
            return handler.HandleRequest(request);
        }
		
	}



}

