using System;
using FruitHAP.Core.Plugin;
using FruitHAP.Common.Configuration;
using Castle.Core.Logging;
using System.IO;
using System.Reflection;
using Microsoft.Owin.Hosting;

namespace FruitHAP.Plugins.Web.Startup
{
	public class WebHost : IPlugin
	{
		private readonly IConfigProvider<WebConfiguration> configurationProvider;
		private readonly ILogger logger;
		private const string CONFIG_FILENAME = "webconfiguration.json";
		private IDisposable owinHost;

		#region IAction implementation

		public void Initialize ()
		{			
			logger.InfoFormat ("Loading configuration");
			var configuration = configurationProvider.LoadConfigFromFile (Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), CONFIG_FILENAME));
			logger.InfoFormat ("************* Starting webserver at {0} ***********",configuration.BaseUrl);
			owinHost = WebApp.Start<WebApplication> (configuration.BaseUrl);

			logger.InfoFormat("***** Webserver started at url {0} ******* ", configuration.BaseUrl);
		}

		#endregion

		#region IDisposable implementation

		public void Dispose ()
		{
			if (owinHost != null) 
			{
				logger.Info ("****** Stopping webserver **********");
				owinHost.Dispose ();
			}
		}

		#endregion

		public WebHost(ILogger logger, IConfigProvider<WebConfiguration> configurationProvider)
		{
			this.logger = logger;
			this.configurationProvider = configurationProvider;
		}
	}
}

