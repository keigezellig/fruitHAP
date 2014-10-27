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

        public PushBulletNotifier(ILogger logger, IPushBulletService pushBulletService) : base(logger)
        {
            this.pushBulletService = pushBulletService;
            pushBulletService.Initialize("v1cULYbIkSKLywzH0b3k5xf3mgsuWyWJE2ujxvzW0Rqay","https://api.pushbullet.com");
        }

        protected override bool ShouldMessageBeProcessed(DoorMessage message)
        {
            return true;
        }

        protected override void ProcessMessage(DoorMessage message)
        {
            string notificationMessage = string.Format("The doorbell rang at {0}. Please go answer it!", message.TimeStamp);
            string title = "DINGDONG";
            pushBulletService.PostNote(title, notificationMessage);
        }
    }
}
