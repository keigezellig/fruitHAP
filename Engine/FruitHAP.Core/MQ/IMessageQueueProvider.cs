using System;
using System.Threading.Tasks;

namespace FruitHAP.Core.MQ
{
	public interface IMessageQueueProvider : IDisposable
	{
		void Publish<T>(T message, string routingKey) where T: class;
		void Initialize(string connectionString, string pubSubExchangeName);
		bool IsIntialized { get;}
																							 
	}
}
	