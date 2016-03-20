using System;
using FruitHAP.Core.Action;
using Microsoft.Owin.Hosting;
using Castle.Core.Logging;
using System.Collections.Generic;
using Owin;
using System.Web.Http;
using FruitHAP.Core.SensorPersister;
using FruitHAP.Core.SensorRepository;
using Castle.Windsor;
using FruitHAP.Core;

namespace FruitHap.RestInterface
{
	public class RestInterfaceAction : IAction
	{
		public static ISensorPersister sensorPersister { get; set; }
		public static ISensorRepository sensorRepository { get; set; }

		private IDisposable owinHost;
		private ILogger logger;

		IWindsorContainer container;

		public RestInterfaceAction (ILogger logger, ISensorPersister persister, ISensorRepository repository)
		{
			this.container = ContainerAccessor.Container;
			this.logger = logger;
			sensorPersister = persister;
			sensorRepository = repository;

		}
		
		#region IAction implementation

		public void Initialize ()
		{
			
			string baseUrl = "http://localhost:8083";
			owinHost = WebApp.Start<Startup> (baseUrl);
				
			logger.InfoFormat("***** Webserver started at url {0} ******* ", baseUrl);
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



	}
}

