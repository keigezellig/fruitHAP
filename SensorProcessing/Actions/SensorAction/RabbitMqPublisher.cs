using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace SensorProcessing.SensorAction
{
    public class RabbitMqPublisher
    {
        private ConnectionFactory factory;

        private const string EasyNetQDeserializableType =
            "EventNotifierService.Common.Messages.DoorMessage:EventNotifierService.Common";

        public RabbitMqPublisher(string connectionString)
        {
            factory = new ConnectionFactory();
            factory.Uri = connectionString;
        }


        public void Publish<T>(string exchange, string routingKey, T message) where T : class
        {
            byte[] payload = GetPayload(message);
            using (IConnection conn = factory.CreateConnection())
            {
                using (IModel model = conn.CreateModel())
                {
                    model.ExchangeDeclare(exchange, "topic",true);
                    IBasicProperties basicProperties = new RabbitMQ.Client.Framing.BasicProperties();
                    basicProperties.DeliveryMode = 2;
                    basicProperties.Type = EasyNetQDeserializableType;
                    model.BasicPublish(exchange,routingKey,basicProperties,payload);
                }
            }         
        }

        private byte[] GetPayload<T>(T message)
        {
            string json = JsonConvert.SerializeObject(message);
            return Encoding.Unicode.GetBytes(json);
        }
    }
}
