using System;
using FruitHAP.Core.Plugin;
using FruitHAP.Common.Configuration;
using Castle.Core.Logging;
using System.IO;
using System.Reflection;
using Microsoft.Owin.Hosting;

namespace FruitHAP.Plugins.Web.Startup
{
    public class WebHost : BasePluginWithConfiguration<WebConfiguration>
	{
		private const string CONFIG_FILENAME = "webconfiguration.json";
		private IDisposable owinHost;

        #region BasePluginWithConfiguration implementation

        protected override void InitializePlugin()
        {
            logger.InfoFormat("************* Starting webserver at {0} ***********", configuration.BaseUrl);
            owinHost = WebApp.Start<WebApplication>(configuration.BaseUrl);

            logger.InfoFormat("***** Webserver started at url {0} ******* ", configuration.BaseUrl);
        }

        protected override void CleanUpPlugin()
        {
            if (owinHost != null)
            {
                logger.Info("****** Stopping webserver **********");
                owinHost.Dispose();
            }
        }

        protected override string GetConfigurationFileName()
        {
            return CONFIG_FILENAME;
        }
       
        #endregion

        public WebHost(ILogger logger, IConfigProvider<WebConfiguration> configurationProvider)
            : base(logger, configurationProvider)
        {
        }

        
		

		

	}
}

