using System;
using EasyNetQ;
using Castle.Core.Logging;

namespace FruitHAP.Core.MQ
{
	public class RabbitMqPublisher : IMessageQueuePublisher
    {
        private IBus messageBus;
		private ILogger logger;

		public RabbitMqPublisher(ILogger logger)
        {
			this.logger = logger;            
        }
			
		public void Initialize(string connectionString)
		{
			logger.InfoFormat ("Connecting to Rabbit MQ with connection string {0}", connectionString);
			messageBus = CreateMessageBus(connectionString);
		}

		public void Publish(object message)
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
