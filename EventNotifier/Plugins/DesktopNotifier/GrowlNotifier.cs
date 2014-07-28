using Castle.Core.Logging;
using EventNotifierService.Common.Messages;
using EventNotifierService.Common.Plugin;
using Growl.Connector;

namespace EventNotifier.Plugins.DesktopNotifier
{
    public class GrowlNotifier : PluginBase
    {
        private readonly GrowlConnector growlConnector;        

        public GrowlNotifier(ILogger logger) : base(logger)
        {            
            growlConnector = new GrowlConnector();
            
        }

        protected override bool ShouldMessageBeProcessed(DoorMessage message)
        {
            return message.EventType == EventType.Ring && GrowlConnector.IsGrowlRunningLocally();

        }

        protected override void ProcessMessage(DoorMessage message)
        {
           logger.Info("Sending message to Growl...");
           SetupGrowl();
            string notificationMessage = string.Format("The doorbell rang at {0}. Please go answer it!",message.TimeStamp);            
            var notification = new Notification("EventNotifier", "DOOR", null, "DINGDONG", notificationMessage);            
            growlConnector.Notify(notification);
        }

        private void SetupGrowl()
        {
            if (!GrowlConnector.IsGrowlRunningLocally())
            {
                logger.Warn("Growl is not running");
            }
            var application = new Application("EventNotifier");
            var doorEventReceived = new NotificationType("DOOR", "Door");
            growlConnector.Register(application, new[] { doorEventReceived });

        }
    }
}
