using Castle.Core.Logging;
using SensorProcessing.Common.Configuration;
using SensorProcessing.Common.Helpers;

namespace SensorProcessing.SensorBinding.RfxBinding
{
    public class RfxBindingConfigurationProvider : ConfigProviderBase<RfxBindingConfiguration>
    {
        public RfxBindingConfigurationProvider(ILogger logger) : base(logger)
        {
        }

        protected override RfxBindingConfiguration LoadFromFile(string fileName)
        {
            return XmlSerializerHelper.Deserialize<RfxBindingConfiguration>(fileName);
        }

        protected override RfxBindingConfiguration LoadDefaultConfig()
        {
            return new RfxBindingConfiguration() {ConnectionString = "serial:COM4,38400,8,N,1"};
        }
    }
}
