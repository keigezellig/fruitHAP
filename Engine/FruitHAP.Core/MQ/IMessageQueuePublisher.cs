using System;

namespace FruitHAP.Core
{
	public interface IMessageQueuePublisher : IDisposable
	{
		void Publish(object message);
		void Initialize(string connectionString);
	}
}

