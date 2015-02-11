using System;

namespace EventNotifierService.Common.Messages
{
	public class DoorMessage
	{
		public DateTime Timestamp { get; set; }
		public EventType EventType { get; set; }
		public string EncodedImage { get; set; }

		public override string ToString ()
		{
			return string.Format ("[DoorMessage: Timestamp={0}, EventType={1}, EncodedImage={2}]", Timestamp, EventType, String.IsNullOrEmpty(EncodedImage) ? "(notset)" : "(set)");
		}

	}

	public enum EventType
	{
		Unknown=0,
		Ring=1
	}
}

