﻿using System;
using Microsoft.Practices.Prism.PubSubEvents;
using Castle.Core.Logging;

namespace FruitHAP.Common.EventBus
{
	public class PrismEventBus : IEventBus
	{
		private readonly IEventAggregator aggregator;
		private readonly ILogger logger;

		public PrismEventBus (IEventAggregator aggregator, ILogger logger )
		{
			this.logger = logger;
			this.aggregator = aggregator;
		}

		public void Publish<TEvent> (TEvent data)
		{			
			var theEvent = aggregator.GetEvent<PrismEvent<TEvent>> ();
			theEvent.Publish (data);
			logger.DebugFormat ("Published event {0}", data);
		}

		public void Subscribe<TEvent> (Action<TEvent> eventHandler, Predicate<TEvent> eventFilter)
		{
			var theEvent = aggregator.GetEvent<PrismEvent<TEvent>> ();
			theEvent.Subscribe(eventHandler,ThreadOption.PublisherThread,true,new Predicate<TEvent>(eventFilter));
            logger.DebugFormat ("Subscribed from event {0}",theEvent);

		}

		public void Subscribe<TEvent> (Action<TEvent> eventHandler)
		{
			var theEvent = aggregator.GetEvent<PrismEvent<TEvent>> ();
			theEvent.Subscribe (eventHandler, ThreadOption.PublisherThread, true);
            logger.DebugFormat ("Subscribed from event {0}",theEvent);
		}

		public void Unsubscribe<TEvent> (Action<TEvent> eventHandler)
		{
			var theEvent = aggregator.GetEvent<PrismEvent<TEvent>> ();
			theEvent.Unsubscribe (eventHandler);
            logger.DebugFormat ("Unsubscribed from event {0}",theEvent);
		}

	}


	public class PrismEvent<TPayload> : PubSubEvent<TPayload>
	{
	}
}

