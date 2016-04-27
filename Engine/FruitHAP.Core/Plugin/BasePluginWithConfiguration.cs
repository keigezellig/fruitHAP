using System;
using Castle.Core.Logging;
using FruitHAP.Common.Configuration;
using System.IO;
using System.Reflection;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Core.Plugin
{
    public abstract class BasePluginWithConfiguration<TConfiguration> : IPlugin  where TConfiguration : BasePluginConfiguration
    {
        protected IConfigProvider<TConfiguration> configurationProvider;
        protected ILogger logger;
        protected TConfiguration configuration;

        protected abstract void InitializePlugin();
        protected abstract void CleanUpPlugin();
        protected abstract string GetConfigurationFileName();        

        #region IPlugin implementation

        public void Initialize()
        {
            logger.InfoFormat ("Initializing plugin {0}", this);
            logger.InfoFormat ("Loading configuration");

            string fullConfigPath = Path.Combine(AssemblyHelpers.GetAssemblyDirectory(this.GetType().Assembly), GetConfigurationFileName());
            configuration = configurationProvider.LoadConfigFromFile(fullConfigPath);
            if (configuration.IsEnabled)
            {               
                InitializePlugin();
            }

            
        }

        public bool IsEnabled
        {
            get { return configuration.IsEnabled; }
        }

        public string Name
        {
            get { return AssemblyHelpers.GetAssemblyTitle(this.GetType().Assembly); }
        }

        public string Description
        {
            get
            {
                return AssemblyHelpers.GetAssemblyDescription(this.GetType().Assembly);            
                
            }
        }
       

        public string Version
        {
            get { return AssemblyHelpers.GetAssemblyVersion(this.GetType().Assembly); }
        }

        

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            logger.DebugFormat("Dispose plugin {0}", this);
            CleanUpPlugin();
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Version);
        }

        protected BasePluginWithConfiguration (ILogger logger, IConfigProvider<TConfiguration> configurationProvider)            
        {
            this.configurationProvider = configurationProvider;
            this.logger = logger;
        }





    }
}

