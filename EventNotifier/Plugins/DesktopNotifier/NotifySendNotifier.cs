using System;
using System.Diagnostics;
using System.IO;
using Castle.Core.Logging;
using EventNotifierService.Common.Messages;
using EventNotifierService.Common.Plugin;

namespace EventNotifier.Plugins.DesktopNotifier
{
    public class NotifySendNotifier : PluginBase
    {
        private readonly IConfigProvider<NotifySendConfiguration> configProvider;
        private readonly NotifySendConfiguration configuration;
        
        public NotifySendNotifier(ILogger logger, IConfigProvider<NotifySendConfiguration> configProvider) : base(logger)
        {
            this.configProvider = configProvider;
            this.name = "NotifySendNotifier";
            configuration = configProvider.LoadConfigFromFile(name + ".xml");
        }

        private bool IsServiceRunningOnLinux()
        {
            int p = (int) Environment.OSVersion.Platform;
            return (p == 4) || (p == 6) || (p == 128);
        }

        protected override bool ShouldMessageBeProcessed(DoorMessage message)
        {
            if (configuration == null)
            {
                logger.Error("No configuration available");
                return false;
            }

            if (!IsServiceRunningOnLinux())
            {
                logger.Warn("Service is not running on a Linux machine");
                return false;
            }


            if (!File.Exists(configuration.FullPathToNotifySendExecutable))
            {
                logger.ErrorFormat("Notify-send executable not found at {0}. Check configuration",
                    configuration.FullPathToNotifySendExecutable);
            }


            return message.EventType == EventType.Ring;
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
            cmdToSend.FileName = string.Format("{0}/notify-send",configuration.FullPathToNotifySendExecutable);
            cmdToSend.Arguments = string.Format("-u normal \"{0}\" \"{1}\"", "DINGDONG", notificationMessage);
            cmdToSend.UseShellExecute = false;
            var process = Process.Start(cmdToSend);
            process.WaitForExit();
            logger.DebugFormat("Exit code notify: {0}", process.ExitCode);
        }
    }
}
