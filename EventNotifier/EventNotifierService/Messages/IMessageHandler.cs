namespace EventNotifierService.Messages
{
    public interface IMessageHandler
	{
        void HandleMessage(DoorMessage message);
	}
}

