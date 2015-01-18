using System.Collections.Generic;
using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using SensorProcessing.Common;
using SensorProcessing.SensorBinding.RfxBinding.Decoders;
using SensorProcessing.SensorBinding.RfxBinding.Decoders.RfxDecoder;
using SensorProcessing.SensorBinding.RfxBinding.Eventing;

namespace SensorProcessing.SensorBinding.RfxBinding
{
    public class RfxProtocolFactory : IRfxProtocolFactory
    {
        private readonly ILogger logger;
        private readonly IEventAggregator eventAggregator;

        public RfxProtocolFactory(ILogger logger, IEventAggregator eventAggregator)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
        }

        public IProtocol CreateRfxProtocol()
        {
            var acDecoder = new AcSubProtocolDecoder(new AcPduPublisher(eventAggregator,logger));
            var lightingTwoDecoder = new LightingTwoProtocolDecoder(new List<IDecoder>() {acDecoder});
            return new RfxProtocol(new List<IDecoder>() {lightingTwoDecoder});
        }
    }
    }
