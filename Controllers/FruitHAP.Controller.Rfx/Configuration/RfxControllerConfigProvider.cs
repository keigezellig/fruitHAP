using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using FruitHAP.Common.Configuration;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Controller.Rfx.Configuration
{
    public class RfxControllerConfigProvider : ConfigProviderBase<RfxControllerConfiguration>

    {
        public RfxControllerConfigProvider(ILogger logger) : base(logger)
        {
        }

        protected override RfxControllerConfiguration LoadFromFile(string fileName)
        {
			return JsonSerializerHelper.Deserialize<RfxControllerConfiguration>(fileName);
        }

        protected override void SaveToFile(string fileName, RfxControllerConfiguration config)
        {
			JsonSerializerHelper.Serialize(fileName,config);
        }

        protected override RfxControllerConfiguration LoadDefaultConfig()
        {
			return new RfxControllerConfiguration() {ConnectionString = "serial:/dev/ttyUSB0,38400,8,N,1"};
        }
       
    }
}
