using System;
using System.Text;
using EasyNetQ;
using FruitHAP.Messages;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace FruitHAP.SensorProcessing.SensorAction
{
    public class RabbitMqPublisher : IDisposable
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
