using EventNotifierService.Common.Messages;
using FruitHAP.Messages;

namespace EventNotifierService.Common.Plugin
{
    public interface IPlugin
	{
        string Name { get; }
        void HandleMessage(DoorMessage message);
	}
}

