using System;
using System.Diagnostics;
using Castle.Core.Logging;
using EventNotifierService.Common.Messages;
using EventNotifierService.Common.Plugin;

namespace EventNotifier.Plugins.DesktopNotifier
{
    public class NotifySendNotifier : PluginBase
    {
        private const string CmdLine = "/usr/bin/notify-send -u normal \"{0}\" \"{1}\" ";
        
        public NotifySendNotifier(ILogger logger) : base(logger)
        {
        }

        private bool IsServiceRunningOnLinux()
        {
            int p = (int) Environment.OSVersion.Platform;
            return (p == 4) || (p == 6) || (p == 128);
        }

        protected override bool CanProcessMessage(DoorMessage message)
        {
            return message.EventType == EventType.Ring && IsServiceRunningOnLinux();
        }

        protected override void ProcessMessage(DoorMessage message)
        {
            logger.Info("Doorbell rang!! Sending notification to desktop");
            ExecuteCommand(message);
        }

       
        private void ExecuteCommand(DoorMessage message)
        {
            string notificationMessage = string.Format("The doorbell rang at {0}. Please go answer it",
                message.TimeStamp);

            ProcessStartInfo cmdToSend = new ProcessStartInfo();
            logger.DebugFormat("cmd to execute: {0}", cmdToSend);
            cmdToSend.FileName = "/usr/bin/notify-send";
            cmdToSend.Arguments = string.Format("-u normal \"{0}\" \"{1}\"", "DINGDONG", notificationMessage);
            cmdToSend.UseShellExecute = false;
            var process = Process.Start(cmdToSend);
            process.WaitForExit();
            logger.DebugFormat("Exit code notify: {0}", process.ExitCode);
        }
    }
}
