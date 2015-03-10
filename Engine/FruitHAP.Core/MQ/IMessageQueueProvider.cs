using System;
using System.Threading.Tasks;

namespace FruitHAP.Core
{
	public interface IMessageQueueProvider : IDisposable
	{
		void Publish<T>(T message, string routingKey) where T: class;
		void Initialize(string connectionString, string pubSubExchangeName, string rpcExchangeName,  string rpcQueueName);
		void SubscribeToRequest<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler) where TRequest : class  
																							  where TResponse : class;
	}
}

