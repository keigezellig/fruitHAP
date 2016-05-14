using System;
using System.IO;
using Castle.Core.Logging;

namespace FruitHAP.Common.Configuration
{
    public abstract class ConfigProviderBase<TConfig> : IConfigProvider<TConfig> where TConfig : class 
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
            TConfig result = null;

            try
            {
                logger.DebugFormat("Loading from file {0}", fileName);
                result = LoadFromFile(fileName);
            }
            catch (FileNotFoundException)
            {
                logger.Error("Configuration file does not exist. Creating default config");
                result = LoadDefaultConfig();
                logger.InfoFormat("Saving default configuration to {0}", fileName);
                SaveConfigToFile(result, fileName);
            }
            catch (Exception ex)
            {                                
                logger.Error("Error occured while loading config from configfile.",ex);                
            }

            return result;

        }

        public void SaveConfigToFile(TConfig config, string fileName)
        {
            SaveToFile(fileName,config);
        }
    }
}
