using System;
using Castle.Core.Logging;
using FruitHAP.Common.Configuration;
using System.IO;
using System.Reflection;

namespace FruitHAP.Core.Plugin
{
    public abstract class BasePluginWithConfiguration<TConfiguration> : IPlugin  where TConfiguration : BasePluginConfiguration
    {
        protected IConfigProvider<TConfiguration> configurationProvider;
        protected ILogger logger;
        protected TConfiguration configuration;

        protected abstract string GetConfigurationFileName();
        protected abstract void InitializePlugin();
        protected abstract void CleanUpPlugin();


        #region IPlugin implementation

        public void Initialize()
        {
            logger.InfoFormat ("Initializing plugin {0}", this);
            logger.InfoFormat ("Loading configuration");
            configuration = configurationProvider.LoadConfigFromFile (GetConfigurationFileName());
            if (configuration.IsEnabled)
            {               
                InitializePlugin();
            }
            else
            {
                logger.WarnFormat("Plugin {0} is NOT enabled", this);
            }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            logger.DebugFormat("Dispose plugin {0}", this);
            CleanUpPlugin();
        }

        #endregion

        protected BasePluginWithConfiguration (ILogger logger, IConfigProvider<TConfiguration> configurationProvider)            
        {
            this.configurationProvider = configurationProvider;
            this.logger = logger;
        }





    }
}

