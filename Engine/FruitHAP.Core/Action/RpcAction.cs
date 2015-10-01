using System;
using Castle.Core.Logging;
using FruitHAP.Core.MQ;
using System.Threading.Tasks;

namespace FruitHAP.Core.Action
{
	public abstract class RpcAction<TRequest, TResponse> : IAction where TRequest : class 
                                                                   where TResponse : class
	{
		protected readonly ILogger logger;
		private readonly IMessageQueueProvider mqProvider;

		protected abstract TResponse ProcessRequest (TRequest request);
		protected virtual void DisposeResources() 
		{
		}

		protected virtual void InitializeResources ()
		{			
		}

		protected RpcAction (ILogger logger, IMessageQueueProvider mqProvider)
		{
			this.logger = logger;
			this.mqProvider = mqProvider;
		}
		
		#region IAction implementation

		public void Initialize ()
		{
			logger.InfoFormat ("Initializing action {0}", this);
			InitializeResources ();
			mqProvider.SubscribeToRequest<TRequest, TResponse> (HandleIncomingRequest);
		}

		#endregion

		#region IDisposable implementation

		public void Dispose ()
		{
			DisposeResources ();
		}

		#endregion

		Task<TResponse> HandleIncomingRequest (TRequest request)
		{
			Task<TResponse> task = 
				new Task<TResponse> (() => 
					{						
						TResponse result = ProcessRequest(request);
						logger.InfoFormat("Sending message {0}",result);
						return result;
					});
			task.Start ();
			return task;
		}



	}
}

