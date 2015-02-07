using System;
using EasyNetQ;
using Castle.Core.Logging;

namespace FruitHAP.Core.MQ
{
	public class RabbitMqProvider : IMessageQueueProvider
    {
        private IBus messageBus;
		private ILogger logger;

		public RabbitMqProvider(ILogger logger)
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
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("MQ connection string is missing");
            }
            return RabbitHutch.CreateBus(connectionString);
        }

        public void Dispose()
        {
            messageBus.Dispose();
        }
    }
}
