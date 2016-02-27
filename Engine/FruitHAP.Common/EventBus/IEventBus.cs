using System;

namespace FruitHAP.Common.EventBus
{
	public interface IEventBus
	{
		void Publish<TEvent>(TEvent data);
		void Subscribe<TEvent> (Action<TEvent> eventHandler, Predicate<TEvent> eventFilter);
		void Subscribe<TEvent> (Action<TEvent> eventHandler);
		void Unsubscribe<TEvent> (Action<TEvent> eventHandler);

	}
}

