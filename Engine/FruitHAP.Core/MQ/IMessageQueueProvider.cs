using System;

namespace FruitHAP.Core
{
	public interface IMessageQueueProvider : IDisposable
	{
		void Publish<T>(T message) where T: class;
		void Initialize(string connectionString);
	}
}

