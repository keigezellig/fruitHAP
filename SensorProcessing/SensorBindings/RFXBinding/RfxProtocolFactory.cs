using System.Collections.Generic;
using Castle.Core.Logging;
using FruitHAP.SensorProcessing.Common;
using FruitHAP.SensorProcessing.SensorBinding.RfxBinding.Decoders;
using FruitHAP.SensorProcessing.SensorBinding.RfxBinding.Decoders.RfxDecoder;
using FruitHAP.SensorProcessing.SensorBinding.RfxBinding.Eventing;
using Microsoft.Practices.Prism.PubSubEvents;

namespace FruitHAP.SensorProcessing.SensorBinding.RfxBinding
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
            var acDecoder = new AcSubProtocolDecoder(new AcPduPublisher(eventAggregator, logger), logger);
            var lightingTwoDecoder = new LightingTwoProtocolDecoder(new List<IDecoder>() {acDecoder}, logger);
            return new RfxProtocol(new List<IDecoder>() {lightingTwoDecoder});
        }
    }
}
