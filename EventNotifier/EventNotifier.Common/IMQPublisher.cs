using System;
using EventNotifierService.Common.Messages;
using FruitHAP.Messages;

namespace EventNotifierService.Common
{
    public interface IMQPublisher : IDisposable
    {
        void Publish(DoorMessage message);
    }
}
