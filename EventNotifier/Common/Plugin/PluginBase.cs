using Castle.Core.Logging;
using EventNotifierService.Common.Messages;

namespace EventNotifierService.Common.Plugin
{
    public abstract class PluginBase : IPlugin
    {
        protected abstract bool CanProcessMessage(DoorMessage message);
        protected abstract void ProcessMessage(DoorMessage message);
        protected readonly ILogger logger;

        protected PluginBase(ILogger logger)
        {
            this.logger = logger ?? NullLogger.Instance;
        }
        
        public void HandleMessage(DoorMessage message)
        {
            if (CanProcessMessage(message))
            {
                ProcessMessage(message);    
            }
            else
            {
                logger.Warn("Plugin cannot handle this message");
            }

        }
    }
}
