using Castle.Core.Logging;
using FruitHAP.Common.Configuration;
using FruitHAP.Controllers.ImageCaptureController.Configuration;
using FruitHAP.Core.Controller;
using FruitHAP.Core.Sensor.PacketData.ImageCapture;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.Controllers.ImageCaptureController
{
    public class LocalImageCaptureController : BaseController
    {
        private readonly IConfigProvider<LocalImageCaptureControllerConfiguration> configProvider;
        private const string CONFIG_FILENAME = "LocalImageCapture.json";
        private LocalImageCaptureControllerConfiguration configuration;
        private List<SubscriptionToken> subscriptionTokens;
        private SubscriptionToken subscriptionToken;

        public LocalImageCaptureController(ILogger logger, IEventAggregator aggregator, IConfigProvider<LocalImageCaptureControllerConfiguration> configProvider) : base(logger, aggregator)
        {
            this.configProvider = configProvider;
        }
        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override void DisposeController()
        {
            UnSubscribe();
        }

        protected override void StartController()
        {
            LoadConfiguration();
            Subscribe();
        }

        private void Subscribe()
        {            
            logger.Debug("Subscribing to sensor request");
            subscriptionToken = aggregator.GetEvent<ImageRequestPacketEvent>().Subscribe(HandleImageRequestPacket, ThreadOption.PublisherThread, true, f => f.Direction == Direction.ToController && f.Payload.Uri.StartsWith("local://"));            
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
            throw new NotImplementedException();

            //aggregator.GetEvent<ImageResponsePacketEvent>().Publish(new ControllerEventData<ImageResponsePacketEvent>())
        }

        private void LoadConfiguration()
        {
            configuration = configProvider.LoadConfigFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), CONFIG_FILENAME));
            if (!configProvider.IsConfigurationCorrect(configuration))
            {
                throw new Exception("Incorrect configuration. Please check it.");
            }
        }

        protected override void StopController()
        {
            throw new NotImplementedException();
        }
    }
}
