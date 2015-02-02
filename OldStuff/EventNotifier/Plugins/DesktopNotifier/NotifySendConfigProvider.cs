using System;
using Castle.Core.Logging;
using EventNotifierService.Common.Helpers;
using EventNotifierService.Common.Plugin;

namespace EventNotifier.Plugins.DesktopNotifier
{
    public class NotifySendConfigProvider : ConfigProviderBase<NotifySendConfiguration>
    {
        public NotifySendConfigProvider(ILogger logger) : base(logger)
        {
            
        }

        protected override NotifySendConfiguration LoadFromFile(string fileName)
        {
            return XmlSerializerHelper.Deserialize<NotifySendConfiguration>(fileName);
        }

        protected override NotifySendConfiguration LoadDefaultConfig()
        {
            return new NotifySendConfiguration() {FullPathToNotifySendExecutable = "/usr/bin"};
        }
    }
}
