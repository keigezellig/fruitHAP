using System;
using EventNotifierService.Common.Messages;

namespace EventNotifierService.Common
{
    public interface IMQPublisher : IDisposable
    {
        void Publish(DoorMessage message);
    }
}
