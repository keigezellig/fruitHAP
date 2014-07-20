using System;
using DoorPi.MessageQueuePublisher;
using EventNotifierService.Common;
using EventNotifierService.Common.Messages;

namespace DoorPi.CliSimulator
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Usage: {0} connection_string ", Environment.GetCommandLineArgs()[0]);
                Environment.Exit(1);
            }

            string connectionString = args[0];

            try
            {
                using (IMQPublisher publisher = new RabbitMqPublisher(connectionString))
                {

                    DoorMessage message = new DoorMessage {EventType = EventType.Ring, TimeStamp = DateTime.Now};
                    publisher.Publish(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured: {0}", ex);
            }


        }
    }
}
