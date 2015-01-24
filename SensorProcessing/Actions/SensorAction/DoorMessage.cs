using System;

namespace SensorProcessing.SensorAction
{
    public class DoorMessage
    {
        public DateTime TimeStamp { get; set; }
        public EventType EventType { get; set; }
        public string EncodedImage { get; set; }

        public override string ToString()
        {
            return string.Format("[DoorMessage: TimeStamp={0}, EventType={1}, EncodedImage={2}]", TimeStamp, EventType, !(string.IsNullOrEmpty(EncodedImage)) ? "(set)" : "(not set)");
        }
    }

    public enum EventType
    {
        Unknown = 0,
        Ring = 1
    }
}