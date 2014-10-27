using Castle.Core.Logging;
using EventNotifierService.Common.Helpers;
using EventNotifierService.Common.Plugin;

namespace EventNotifier.Plugins.PushBullet
{
    public class PushbulletConfigProvider :  ConfigProviderBase<PushbulletConfiguration>
    {
        public PushbulletConfigProvider(ILogger logger)
            : base(logger)
        {
            
        }

        protected override PushbulletConfiguration LoadFromFile(string fileName)
        {            
            var result = XmlSerializerHelper.Deserialize<PushbulletConfiguration>(fileName);
            logger.DebugFormat("Url={0}, ApiKey={1}",result.PushbulletUri, result.ApiKey);
            return result;
        }

        protected override PushbulletConfiguration LoadDefaultConfig()
        {
            return new PushbulletConfiguration {PushbulletUri = "https://api.pushbullet.com"};
        }
    }
}
