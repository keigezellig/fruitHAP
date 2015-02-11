using System;
using EasyNetQ;
using Castle.Core.Logging;
using EasyNetQ.Topology;
using FruitHAP.Common.Helpers;
using System.Text;


namespace FruitHAP.Core.MQ
{
	public class RabbitMqProvider : IMessageQueueProvider
    {
        private IBus messageBus;
		private ILogger logger;
		private IExchange exchange;

		public RabbitMqProvider(ILogger logger)
        {
			this.logger = logger;            
        }
			
		public void Initialize(string connectionString, string exchangeName)
		{
			logger.InfoFormat ("Connecting to Rabbit MQ with connection string {0} and to exchange {1}", connectionString,exchangeName);
			messageBus = CreateMessageBus(connectionString);
			exchange = messageBus.Advanced.ExchangeDeclare (exchangeName, ExchangeType.Topic,false,false,false,false,null);
		}

		public void Publish<T>(T message, string routingKey) where T: class
        {
			string json = message.ToJsonString();
			byte[] bytes = Encoding.UTF8.GetBytes (json);
			MessageProperties properties = new MessageProperties ();
			properties.ContentType = "text/json";
			properties.ContentTypePresent = true;

			messageBus.Advanced.Publish (exchange, routingKey, false, false, properties, bytes);
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
