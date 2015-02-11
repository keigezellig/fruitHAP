using EventNotifierService.Common.Messages;

namespace EventNotifierService.Common.Plugin
{
    public interface IPlugin
	{
        string Name { get; }
        void HandleMessage(DoorMessage message);
	}
}

