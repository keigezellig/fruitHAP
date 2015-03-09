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
		private IExchange exchange;

		public RabbitMqProvider(ILogger logger)
        {
			this.logger = logger;            
        }
			
		public void Initialize(string connectionString, string pubSubExchangeName, string rpcExchangeName, string rpcQueueName)
		{
			logger.InfoFormat ("Connecting to Rabbit MQ with connection string {0} and to exchange {1} and RPC queue {2}", connectionString,pubSubExchangeName,rpcQueueName );
			messageBus = CreateMessageBus(connectionString, rpcExchangeName, rpcQueueName);
			exchange = messageBus.Advanced.ExchangeDeclare (pubSubExchangeName, ExchangeType.Topic,false,false,false,false,null);
		}

		public void Publish<T>(T message, string routingKey) where T: class
        {
			var mqMessage = new Message<T> (message);
			messageBus.Advanced.Publish (exchange, routingKey, false, false, mqMessage);

        }

		public void SubscribeToRequest<TRequest, TResponse>(Func<TRequest, TResponse> handler) 
			where TRequest : class  
			where TResponse : class
		{
			try
			{
				messageBus.Respond<TRequest,TResponse> (handler);
			}
			catch (Exception ex) 
			{
				throw;
			}
		}

		private IBus CreateMessageBus(string connectionString, string exchangeName, string rpcQueueName)
        {
			if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("MQ connection string is missing");
            }
			var bus = RabbitHutch.CreateBus (connectionString, RegisterMyServices);

			SetCorrectMQNamingConventions (bus, exchangeName, rpcQueueName);

			return bus;        
		}

		public void SetCorrectMQNamingConventions (IBus bus, string exchangeName, string rpcQueueName)
		{
			var conventions = bus.Advanced.Container.Resolve<IConventions> ();
			conventions.RpcExchangeNamingConvention = () => exchangeName;
			conventions.RpcRoutingKeyNamingConvention = type => rpcQueueName;
		}
        
		private void RegisterMyServices(IServiceRegister serviceRegister)
		{
			//serviceRegister.Register<ITypeNameSerializer,FruitHAP.Core.MQ.Helpers.TypeNameSerializer> ();

		}

		public void Dispose()
        {
            messageBus.Dispose();
        }
    }
}
