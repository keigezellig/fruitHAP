using Castle.Core.Logging;
using EventNotifier.Plugins.PushBullet.Annotations;
using EventNotifierService.Common.Messages;
using EventNotifierService.Common.Plugin;

namespace EventNotifier.Plugins.PushBullet
{
    [UsedImplicitly]
    public class PushBulletNotifier : PluginBase
    {
        private readonly IPushBulletService pushBulletService;
        private PushbulletConfiguration config;

        public PushBulletNotifier(ILogger logger, IPushBulletService pushBulletService, IConfigProvider<PushbulletConfiguration> configProvider) : base(logger)
        {
            name = "Pushbullet";            
            this.pushBulletService = pushBulletService;
            config = configProvider.LoadConfigFromFile(name + ".xml");            
            pushBulletService.Initialize(config.ApiKey, config.PushbulletUri);            
        }

        protected override bool ShouldMessageBeProcessed(DoorMessage message)
        {
            return true;
        }

        protected override void ProcessMessage(DoorMessage message)
        {
            string notificationMessage = string.Format("The doorbell rang at {0}. Please go answer it!", message.TimeStamp);
            string title = "DINGDONG";
            pushBulletService.PostNote(title, notificationMessage,config.Channel);
        }
    }
}
