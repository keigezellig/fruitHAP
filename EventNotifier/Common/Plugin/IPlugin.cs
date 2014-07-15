using EventNotifierService.Common.Messages;

namespace EventNotifierService.Common.Plugin
{
    public interface IPlugin
	{
        void HandleMessage(DoorMessage message);
	}
}

