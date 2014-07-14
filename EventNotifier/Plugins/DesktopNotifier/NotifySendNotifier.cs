using System;
using System.Diagnostics;
using Castle.Core.Logging;
using EventNotifierService.Messages;

namespace EventNotifier.Plugins.DesktopNotifier
{
    public class NotifySendNotifier : IMessageHandler
    {
        private readonly ILogger logger;
        private const string CmdLine = "/usr/bin/notify-send -u normal \"{0}\" \"{1}\" ";
        
        public NotifySendNotifier(ILogger logger)
        {
            this.logger = logger ?? NullLogger.Instance;
            
        }
        public void HandleMessage(DoorMessage message)
        {
            
            if (message.EventType == EventType.Ring)
            {
                logger.Info("Doorbell rang!! Sending notification to desktop");
                ExecuteCommand(message);    
            }
        }

        private void ExecuteCommand(DoorMessage message)
        {
            string notificationMessage = string.Format("The doorbell rang at {0}. Please go answer it!",
                message.TimeStamp);
            var process = Process.Start(string.Format(CmdLine, "DINGDONG", notificationMessage));
            process.WaitForExit();
            logger.DebugFormat("Exit code notify: {0}", process.ExitCode);
        }
    }
}
