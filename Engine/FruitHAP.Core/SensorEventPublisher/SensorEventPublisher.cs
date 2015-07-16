using System;
using FruitHAP.Core.Sensor;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Collections.Generic;

namespace FruitHAP.Core.SensorEventPublisher
{
	public class SensorEventPublisher : ISensorEventPublisher
	{
		private readonly IEventAggregator aggregator;

		public SensorEventPublisher (IEventAggregator aggregator)
		{
			this.aggregator = aggregator;
		}

		#region ISensorEventPublisher implementation

		public void Publish<TEventType> (ISensor sender, object optionalData) where TEventType : PubSubEvent<EventData>, new()
		{
			var data = new EventData () {TimeStamp = DateTime.Now, 
				EventName = typeof(TEventType).Name,
				Sender = sender,
				OptionalData = optionalData 
			};

			aggregator.GetEvent<TEventType> ().Publish (data);
		}


		public void Subscribe<TEventType> (Action<EventData> handler, Predicate<EventData> filter) where TEventType : PubSubEvent<EventData>, new()
		{
			var @event = aggregator.GetEvent<TEventType> ();
			@event.Subscribe(handler,ThreadOption.PublisherThread,true,filter);
		}

		public SubscriptionToken SubscribeWithToken<TEventType> (Action<EventData> handler, Predicate<EventData> filter) where TEventType : PubSubEvent<EventData>, new()
		{
			var @event = aggregator.GetEvent<TEventType> ();
			return @event.Subscribe(handler,ThreadOption.PublisherThread,true,filter);

		}

		public void Unsubscribe<TEventType> (Action<EventData> handler) where TEventType : PubSubEvent<EventData>, new()
		{
			var @event = aggregator.GetEvent<TEventType> ();
			@event.Unsubscribe (handler);
		}

		public void Unsubscribe<TEventType> (SubscriptionToken token) where TEventType : PubSubEvent<EventData>, new()
		{
			var @event = aggregator.GetEvent<TEventType> ();
			@event.Unsubscribe (token);
		}
		#endregion


	}


}

