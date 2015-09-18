using System;
using Castle.Core.Logging;

namespace FruitHAP.Common.Configuration
{
    public abstract class ConfigProviderBase<TConfig> : IConfigProvider<TConfig>
    {
        protected readonly ILogger logger;

        protected ConfigProviderBase(ILogger logger)
        {
            this.logger = logger ?? NullLogger.Instance;
        }
        protected abstract TConfig LoadFromFile(string fileName);
        protected abstract void SaveToFile(string fileName, TConfig config);

        protected virtual TConfig LoadDefaultConfig()
        {
            return default(TConfig);
        }

        public virtual bool IsConfigurationCorrect(TConfig configuration)
        {
            return true;
        }

        public TConfig LoadConfigFromFile(string fileName)
        {
            var result = default(TConfig);
            
            try
            {
                logger.DebugFormat("Loading from file {0}", fileName);
                result = LoadFromFile(fileName);        
            }
            catch (Exception ex)
            {
                logger.Error("Error occured while loading config from configfile. Loading default config", ex);
                result = LoadDefaultConfig();
				logger.InfoFormat("Saving default configuration to {0}",fileName);
				SaveConfigToFile(result, fileName);
            }

            return result;

        }

        public void SaveConfigToFile(TConfig config, string fileName)
        {
            SaveToFile(fileName,config);
        }
    }
}
