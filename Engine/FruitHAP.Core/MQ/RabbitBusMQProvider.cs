using System;
using Castle.Core.Logging;
using EasyNetQ;
using System.Threading.Tasks;

namespace FruitHAP.Core
{
	public class RabbitBusMQProvider : IMessageQueueProvider
	{
		private IBus messageBus;
		private ILogger logger;

		public RabbitBusMQProvider(ILogger logger)
		{
			this.logger = logger;            
		}


		public void Initialize (string connectionString, string exchangeName, string rpcQueueName)
		{
			logger.InfoFormat ("Connecting to Rabbit MQ with connection string {0}", connectionString);
			messageBus = CreateMessageBus(connectionString,exchangeName, rpcQueueName);
		}

		public void SubscribeToRequest<TRequest, TResponse> (Func<TRequest, Task<TResponse>> handler)
		{
			messageBus.RespondAsync<TRequest,TResponse> (handler);			
		}

		public void Publish<T> (T message, string routingKey) where T : class
		{
			messageBus.Publish(message,routingKey);
		}

		private IBus CreateMessageBus(string connectionString, string exchangeName)
		{
			IBus bus;

			try 
			{

				if (string.IsNullOrEmpty (connectionString)) 
				{
					throw new Exception ("MQ connection string is missing");
				}
				bus = RabbitHutch.CreateBus(connectionString);
				var conventions = bus.Advanced.Container.Resolve<IConventions>();
				conventions.ExchangeNamingConvention = ((messageType) => exchangeName);
				return bus;
			} 
			catch (Exception ex) 
			{
				logger.FatalFormat ("Error while setting up message bus: {0}", ex);
				throw;
			}
		}

		public void Dispose()
		{
			messageBus.Dispose ();
		}
	}
}

