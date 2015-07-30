using System;
using FruitHAP.Core.Sensor;
using Microsoft.Practices.Prism.PubSubEvents;

namespace FruitHAP.Core.SensorEventPublisher
{
	public interface ISensorEventPublisher
	{
		void Publish<TEventType>(ISensor sender, object optionalData = null, DataType dataType = DataType.NoData) where TEventType : PubSubEvent<EventData>, new();
		void Subscribe<TEventType> (Action<EventData> handler, Predicate<EventData> filter) where TEventType : PubSubEvent<EventData>, new();
		SubscriptionToken SubscribeWithToken<TEventType> (Action<EventData> handler, Predicate<EventData> filter) where TEventType : PubSubEvent<EventData>, new();
		void Unsubscribe<TEventType> (Action<EventData> handler) where TEventType : PubSubEvent<EventData>, new();
		void Unsubscribe<TEventType> (SubscriptionToken token) where TEventType : PubSubEvent<EventData>, new();
	}

	public class EventData
	{
		public DateTime TimeStamp { get; set; }
		public string EventType {get; set;}
		public ISensor Sender {get; set;}
		public object OptionalData { get; set; }
        public DataType DataType { get; set; }
	}

    public enum DataType
    {
        NoData = 0,
        BinaryBase64 = 1,
        Binary = 2,
        Json = 3
    }

	public class SensorEvent : PubSubEvent<EventData>
	{
	}
}

