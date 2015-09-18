using Castle.Core.Logging;
using FruitHAP.Core.Controller;
using FruitHAP.Core.Sensor.PacketData.ImageCapture;
using Microsoft.Practices.Prism.PubSubEvents;
using System;

namespace FruitHAP.Controllers.ImageCaptureController
{
    public class ImageCaptureController : BaseController
    {
        private SubscriptionToken subscriptionToken;

        public ImageCaptureController(ILogger logger, IEventAggregator aggregator) : base(logger, aggregator)
        {
        }
        public override string Name
        {
            get
            {
                return "Image capture controller";
            }
        }

        protected override void DisposeController()
        {
            UnSubscribe();
        }

        protected override void StartController()
        {
            Subscribe();
        }

        private void Subscribe()
        {            
            logger.Debug("Subscribing to sensor request");
            subscriptionToken = aggregator.GetEvent<ImageRequestPacketEvent>().Subscribe(HandleImageRequestPacket, ThreadOption.PublisherThread, true, f => f.Direction == Direction.ToController);            
        }

        private void UnSubscribe()
        {
            if (subscriptionToken != null)
            {
                logger.Debug("Unsubscribing from sensor request");
                aggregator.GetEvent<ImageRequestPacketEvent>().Unsubscribe(subscriptionToken);
            }

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

                aggregator.GetEvent<ImageResponsePacketEvent>().Publish(new ControllerEventData<ImageResponsePacket>()
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

       


        protected override void StopController()
        {
			UnSubscribe ();
        }
    }
}
