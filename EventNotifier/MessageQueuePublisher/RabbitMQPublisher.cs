using System;
using EasyNetQ;
using EventNotifierService.Common;
using EventNotifierService.Common.Messages;

namespace DoorPi.MessageQueuePublisher
{
    public class RabbitMqPublisher : IMQPublisher
    {
        private IBus messageBus;

        public RabbitMqPublisher(string connectionString)
        {
            messageBus = CreateMessageBus(connectionString);
        }
        
        
        public void Publish(DoorMessage message)
        {
            messageBus.Publish(message);
        }


        private IBus CreateMessageBus(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("MQ connection string is missing");
            }
            return RabbitHutch.CreateBus(connectionString);
        }

        public void Dispose()
        {
            messageBus.Dispose();
        }
    }
}
