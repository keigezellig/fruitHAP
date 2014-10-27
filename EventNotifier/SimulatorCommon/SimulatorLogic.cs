using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using DoorPi.MessageQueuePublisher;
using EventNotifierService.Common;
using EventNotifierService.Common.Messages;

namespace SimulatorCommon
{
    public class SimulatorLogic
    {
        public static void PublishRingMessage(string connectionString, string imagePath)
        {
            DoorMessage message = new DoorMessage { EventType = EventType.Ring, TimeStamp = DateTime.Now };

            if (!string.IsNullOrEmpty(imagePath))
            {
                message.EncodedImage = EncodeImage(imagePath);
               // DecodeImage(message.EncodedImage);

            }
            using (IMQPublisher publisher = new RabbitMqPublisher(connectionString))
            {
                for (int i = 0; i < 100; i++)
                {
                    publisher.Publish(message);
                }
            }
        }

        private static string EncodeImage(string imagePath)
        {
            var image = Image.FromFile(imagePath);            
            string base64String = string.Empty;

            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, image.RawFormat);            

            memoryStream.Position = 0;
            byte[] byteBuffer = memoryStream.ToArray();

            memoryStream.Close();

            base64String = Convert.ToBase64String(byteBuffer);
            
            return base64String; 
        }

        private static void DecodeImage(string base64String)
        {
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            using (MemoryStream memoryStream = new MemoryStream(byteBuffer))
            {
                var bmpReturn = Image.FromStream(memoryStream);
                bmpReturn.Save(@"C:\develop\decoded",ImageFormat.Jpeg);
            }
        }
    }
}
