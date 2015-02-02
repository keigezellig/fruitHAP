using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using FruitHAP.Common.Configuration;
using FruitHAP.Common.Helpers;

namespace FruitHAP.SensorModule.Kaku.Configuration
{
    public class KakuConfigProvider : ConfigProviderBase<KakuConfiguration>

    {
        public KakuConfigProvider(ILogger logger) : base(logger)
        {
        }

        protected override KakuConfiguration LoadFromFile(string fileName)
        {
            return XmlSerializerHelper.Deserialize<KakuConfiguration>(fileName);
        }

        protected override void SaveToFile(string fileName, KakuConfiguration config)
        {
            XmlSerializerHelper.Serialize(fileName,config);
        }

        protected override KakuConfiguration LoadDefaultConfig()
        {
            return new KakuConfiguration() {ConnectionString = "serial:COM4,38400,8,N,1"};
        }
       
    }
}
