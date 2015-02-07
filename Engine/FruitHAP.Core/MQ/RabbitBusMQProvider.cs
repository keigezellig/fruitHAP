using System;
using Castle.Core.Logging;
using RabbitBus;

namespace FruitHAP.Core
{
	public class RabbitBusMQProvider : IMessageQueuePublisher
	{
		private IBus messageBus;
		private ILogger logger;

		public RabbitBusMQProvider(ILogger logger)
		{
			this.logger = logger;            
		}

		public void Initialize(string connectionString)
		{
			logger.InfoFormat ("Connecting to Rabbit MQ with connection string {0}", connectionString);
			messageBus = CreateMessageBus(connectionString);
		}

		public void Publish<T>(T message) where T: class
		{
			messageBus.Publish(message);
		}


		private IBus CreateMessageBus(string connectionString)
		{
			try 
			{
				if (string.IsNullOrEmpty (connectionString)) 
				{
					throw new Exception ("MQ connection string is missing");
				}

				var bus = new BusBuilder ().Build (); 
				bus.Connect (connectionString);
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

		}


		/*us bus = new BusBuilder()
  /*.Configure(ctx => ctx.Publish<StatusUpdate>()
                         .WithExchange("status-update-exchange"))
  *.Build();
bus.Connect();*/

	}
}

