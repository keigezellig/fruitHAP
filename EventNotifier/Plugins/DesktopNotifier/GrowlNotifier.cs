using System;
using Castle.Core.Logging;
using EventNotifierService.Messages;
using Growl.Connector;

namespace EventNotifier.Plugins.DesktopNotifier
{
    public class GrowlNotifier : IMessageHandler
    {
        private readonly GrowlConnector growlConnector;
        private readonly ILogger logger;

        public GrowlNotifier(ILogger logger)
        {
            this.logger = logger;
            growlConnector = new GrowlConnector();
        }

        public void HandleMessage(DoorMessage message)
        {
           logger.Info("Sending message to Growl...");
            SetupGrowl();
            
            if (message.EventType != EventType.Ring) 
                return;

            string notificationMessage = string.Format("The doorbell rang at {0}. Please go answer it!",message.TimeStamp);            
            var notification = new Notification("EventNotifier", "DOOR", null, "DINGDONG", notificationMessage);            
            growlConnector.Notify(notification);
        }

        private void SetupGrowl()
        {
            var application = new Application("EventNotifier");
            var doorEventReceived = new NotificationType("DOOR", "Door");
            growlConnector.Register(application, new[] { doorEventReceived });

        }
    }
}
