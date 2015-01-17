using System;
using Castle.Core.Logging;

namespace SensorProcessing.Common.Configuration
{
    public abstract class ConfigProviderBase<TConfig> : IConfigProvider<TConfig>
    {
        protected readonly ILogger logger;

        protected ConfigProviderBase(ILogger logger)
        {
            this.logger = logger ?? NullLogger.Instance;
        }
        protected abstract TConfig LoadFromFile(string fileName);

        protected virtual TConfig LoadDefaultConfig()
        {
            return default(TConfig);
        }

        public TConfig LoadConfigFromFile(string fileName)
        {
            try
            {
                logger.DebugFormat("Loading from file {0}", fileName);
                return LoadFromFile(fileName);
            }
            catch (Exception ex)
            {
                logger.Warn("Cannnot load config from configfile. Loading default config", ex);
                return LoadDefaultConfig();
            }
        }

        public void SaveConfigToFile(TConfig config, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
