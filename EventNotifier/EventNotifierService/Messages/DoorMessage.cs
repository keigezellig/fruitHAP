using System;
using EasyNetQ;

namespace EventNotifierService.Messages
{
    [Queue("DoorMessagesQueue", ExchangeName = "EventExchange")]
    public class DoorMessage
    {
        public DateTime TimeStamp {get; set;}
        public EventType EventType { get; set; }

        public override string ToString()
        {
            return string.Format("[DoorMessage: TimeStamp={0}, EventType={1}]", TimeStamp, EventType);
        }
    }
        
    public enum EventType
    {
        Unknown = 0,
        Ring = 1
    }
}

