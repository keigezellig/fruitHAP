using System;
using EasyNetQ;
using Castle.Core.Logging;
using EasyNetQ.Topology;
using FruitHAP.Common.Helpers;
using System.Text;
using System.Threading.Tasks;


namespace FruitHAP.Core.MQ
{
	public class RabbitMqProvider : IMessageQueueProvider
    {
        private IBus messageBus;
		private ILogger logger;
		private IExchange pubsubExchange;
		private bool isInitialized;


		public RabbitMqProvider(ILogger logger)
        {
			this.logger = logger;   
			this.isInitialized = false;
        }


		public bool IsIntialized {
			get 
			{
				return isInitialized;
			}
		}
			
		public void Initialize(string connectionString, string pubSubExchangeName)
		{
			logger.InfoFormat ("Connecting to Rabbit MQ with connection string {0} and to pubsub exchange {1}", connectionString,pubSubExchangeName);
			messageBus = CreateMessageBus (connectionString);
			pubsubExchange = messageBus.Advanced.ExchangeDeclare (pubSubExchangeName, ExchangeType.Topic,false,false,false,false,null);

		}

		public void Publish<T>(T message, string routingKey) where T: class
        {
			var mqMessage = new Message<T> (message);
			messageBus.Advanced.Publish (pubsubExchange, routingKey, false, false, mqMessage);
			logger.InfoFormat ("Message published to MQ. Routing key: {0}",routingKey);
			logger.DebugFormat ("Message published to MQ. Routing key: {0}. Message {1}",routingKey,message);
        }


		private IBus CreateMessageBus(string connectionString)
        {
			if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("MQ connection string is missing");
            }
			var bus = RabbitHutch.CreateBus (connectionString);

			return bus;        
		}

		public void Dispose()
        {
            messageBus.Dispose();
        }
    }
}
