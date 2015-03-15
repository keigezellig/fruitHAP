using System;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Action;
using FruitHAP.Core;
using System.Threading.Tasks;
using FruitHap.MyActions.Messages;
using FruitHAP.Core.Sensor.SensorTypes;

namespace FruitHAP.MyActions
{
    public class DoorbellAction : IAction
    {
		private readonly ISensorRepository sensoRepository;
        private readonly ILogger logger;
		private readonly IMessageQueueProvider mqPublisher;

        public DoorbellAction(ISensorRepository deviceRepository, ILogger logger, IMessageQueueProvider publisher)
        {
            this.sensoRepository = deviceRepository;
            this.logger = logger;
			this.mqPublisher = publisher;
            //amqp://userName:password@hostName:portNumber/virtualHost
            
        }

        public void Initialize()
        {
			logger.InfoFormat ("Initializing action {0}", this);
			IButton doorbellButton = sensoRepository.FindDeviceOfTypeByName<IButton>("Doorbell");
            doorbellButton.ButtonPressed += doorbellButton_ButtonPressed;
        }

        private async void doorbellButton_ButtonPressed(object sender, EventArgs e)
		{
			logger.Info ("Doorbell rang. Creating notification");
			var message = await CreateMessage ();
			logger.DebugFormat ("Message = {0}", message);

			try {
				logger.Info ("Send notification");
				mqPublisher.Publish (message,"alerts");
			} catch (Exception ex) {
				logger.Error ("Error sending notification", ex);
			}
		}

        private async Task<DoorMessage> CreateMessage()
        {
            byte[] image = null;
			DoorMessage message = new DoorMessage() {Timestamp = DateTime.Now, EventType = EventType.Ring};

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
            ICamera doorCamera = sensoRepository.FindDeviceOfTypeByName<ICamera>("DoorCamera");
            return await doorCamera.GetImageAsync();
        }
    }

   

}
