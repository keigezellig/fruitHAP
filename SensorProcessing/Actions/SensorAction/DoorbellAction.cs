using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using SensorProcessing.Common;
using SensorProcessing.Common.Device;

namespace SensorProcessing.SensorAction
{
    public class DoorbellAction : ISensorAction
    {
        private readonly IDeviceRepository deviceRepository;
        private readonly ILogger logger;
        private RabbitMqPublisher mqPublisher;

        public DoorbellAction(IDeviceRepository deviceRepository, ILogger logger)
        {
            this.deviceRepository = deviceRepository;
            this.logger = logger;
            //amqp://userName:password@hostName:portNumber/virtualHost
            mqPublisher = new RabbitMqPublisher("amqp://admin:admin@ljubljana");
        }

        public void Initialize()
        {
            IButton doorbellButton = deviceRepository.FindDeviceOfTypeByName<IButton>("Doorbell");
            doorbellButton.ButtonPressed += doorbellButton_ButtonPressed;
        }

        private async void doorbellButton_ButtonPressed(object sender, EventArgs e)
        {
            logger.Info("Doorbell rang. Creating notification");
            var message = await CreateMessage();
            logger.DebugFormat("Message = {0}", message);

            try
            {
                logger.Info("Send notification");
                mqPublisher.Publish("EventExchange","",message);
            }
            catch (Exception ex)
            {
                logger.Error("Error sending notification",ex);
            }
        }

        private async Task<DoorMessage> CreateMessage()
        {
            byte[] image = null;
            DoorMessage message = new DoorMessage() {TimeStamp = DateTime.Now, EventType = EventType.Ring};

            try
            {
                logger.Info("Retrieving camera image.");
                image = await GetImageFromCamera();
                logger.Info("Retrieving camera image.");
            }
            catch (Exception ex)
            {
                logger.Warn("Could not retrieve camera image. Notification will be send  without image", ex);
            }

            if (image != null)
            {
                message.EncodedImage = Convert.ToBase64String(image);
            }
            
            return message;
        }

        private async Task<byte[]> GetImageFromCamera()
        {
            ICamera doorCamera = deviceRepository.FindDeviceOfTypeByName<ICamera>("DoorCamera");
            return await doorCamera.GetImageAsync();
        }
    }

   

}
