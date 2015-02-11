using System.Security.Cryptography.X509Certificates;
using Castle.Core.Logging;
using EventNotifierService.Common.Messages;
namespace EventNotifierService.Common.Plugin
{
    public abstract class PluginBase : IPlugin
    {
        protected abstract bool ShouldMessageBeProcessed(DoorMessage message);
        protected abstract void ProcessMessage(DoorMessage message);
        protected readonly ILogger logger;
        protected string name;

        protected PluginBase(ILogger logger)
        {
            this.logger = logger ?? NullLogger.Instance;
        }
        
        public void HandleMessage(DoorMessage message)
        {
            if (ShouldMessageBeProcessed(message))
            {
                ProcessMessage(message);    
            }
            else
            {
                logger.Warn("Plugin cannot handle this message");
            }

        }

        public string Name
        {
            get { return name; }
        }
    }
}
