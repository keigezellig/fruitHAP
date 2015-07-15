using FruitHAP.Core.Action;
using FruitHAP.Core.SensorRepository;
using Castle.Core.Logging;
using FruitHAP.Core.MQ;
using System.Threading.Tasks;
using FruitHap.StandardActions.Messages;

namespace FruitHap.StandardActions.IncomingRequest
{
	public class IncomingRequestAction : IAction
	{
		private readonly ISensorRepository sensorRepository;
		private readonly ILogger logger;
		private readonly IMessageQueueProvider mqProvider;
		private readonly IRequestHandlerFactory requestHandlerFactory;

		public IncomingRequestAction(ISensorRepository sensorRepository, ILogger logger, IMessageQueueProvider publisher)
		{
			this.sensorRepository = sensorRepository;
			this.logger = logger;
			this.mqProvider = publisher;
			this.requestHandlerFactory = new RequestHandlerFactory(logger,sensorRepository);
		}


		#region IAction implementation

		public void Initialize ()
		{
			logger.InfoFormat ("Initializing action {0}", this);
			mqProvider.SubscribeToRequest<SensorMessage, SensorMessage> (HandleIncomingRequest);
		}

		#endregion

		private Task<SensorMessage> HandleIncomingRequest (SensorMessage request)
		{
			Task<SensorMessage> task = 
				new Task<SensorMessage> (() => 
					{
						IRequestHandler handler = requestHandlerFactory.GetRequestHandler(request);
						var result = handler.HandleRequest(request);
						logger.InfoFormat("Sending message {0}",result);
						return result;
					});
			task.Start ();
			return task;
		}

		public void Dispose ()
		{
		}
	}



}

