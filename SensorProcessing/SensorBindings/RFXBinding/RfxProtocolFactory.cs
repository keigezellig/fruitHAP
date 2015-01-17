using System.Collections.Generic;
using Castle.Core.Logging;
using SensorProcessing.Common;
using SensorProcessing.SensorBinding.RfxBinding.Decoders;
using SensorProcessing.SensorBinding.RfxBinding.Decoders.RfxDecoder;
using SensorProcessing.SensorBinding.RfxBinding.Eventing;

namespace SensorProcessing.SensorBinding.RfxBinding
{
    public class RfxProtocolFactory : IRfxProtocolFactory
    {
        private readonly ILogger logger;

        public RfxProtocolFactory(ILogger logger)
        {
            this.logger = logger;
        }

        public IProtocol CreateRfxProtocol()
        {
            var acDecoder = new AcSubProtocolDecoder(new AcPduPublisher(logger));
            var lightingTwoDecoder = new LightingTwoProtocolDecoder(new List<IDecoder>() {acDecoder});
            return new RfxProtocol(new List<IDecoder>() {lightingTwoDecoder});
        }
    }
    }
