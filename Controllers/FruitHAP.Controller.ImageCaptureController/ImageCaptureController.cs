using Castle.Core.Logging;
using FruitHAP.Core.Controller;
using FruitHAP.Core.Sensor.PacketData.ImageCapture;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using FruitHAP.Common.EventBus;

namespace FruitHAP.Controllers.ImageCaptureController
{
	public class ImageCaptureController : BaseController
    {

		public ImageCaptureController(ILogger logger, IEventBus eventBus) : base(logger, eventBus)
        {
        }
        public override string Name
        {
            get
            {
                return "Image capture controller";
            }
        }

        protected override void StopController()
        {
            UnSubscribe();
        }

        protected override void StartController()
        {
            Subscribe();
        }

		#region implemented abstract members of BaseController

		protected override void DisposeController ()
		{
			//UnSubscribe();
		}

		#endregion

        private void Subscribe()
        {            
			eventBus.Subscribe<ControllerEventData<ImageRequestPacket>>(HandleImageRequestPacket, f => f.Direction == Direction.ToController);            
        }

        private void UnSubscribe()
        {
			eventBus.Unsubscribe<ControllerEventData<ImageRequestPacket>> (HandleImageRequestPacket);
		}

		

        private void HandleImageRequestPacket(ControllerEventData<ImageRequestPacket> request)
        {
            IImageCapturer imageCapturer = CreateImageCapturer(request.Payload.Uri);
            if (imageCapturer == null)
            {
                logger.Error("Invalid URI specified in request");
                return;
            }

            try
            {
                if (!imageCapturer.IsRequestOk(request.Payload))
                {
                    //TODO: Introduce validator class
					logger.ErrorFormat("Invalid request for {0}",imageCapturer.GetType().Name);
                    return;
                }

                var image = imageCapturer.Capture(request.Payload);

                eventBus.Publish(new ControllerEventData<ImageResponsePacket>()
                {
                    Direction = Direction.FromController,
                    Payload = new ImageResponsePacket() { DestinationSensor = request.Payload.Sender, ImageData = image }
                });
            }
            catch (Exception ex)
            {
                logger.Error("Error capturing image: ", ex);
                throw;
            }
		}

        private IImageCapturer CreateImageCapturer(string uri)
        {
            if (uri.StartsWith("local://"))
            {
                return new LocalImageCapturer(logger);
            }

            if (uri.StartsWith("http://"))
            {
                return new HttpImageCapturer();
            }

            return null;
        }

    }
}
