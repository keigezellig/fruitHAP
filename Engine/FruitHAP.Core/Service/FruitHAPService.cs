using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using FruitHAP.Core.Plugin;
using FruitHAP.Core.Sensor;
using System.Configuration;
using System;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.Controller;
using FruitHAP.Core.MQ;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Core.Service
{
    public class FruitHAPService : IFruitHAPService
    {
        private readonly ISensorRepository sensorRepository;
        private readonly IEnumerable<IController> controllers;
        private readonly IEnumerable<IPlugin> plugins;
        private readonly ILogger log;
		private readonly IMessageQueueProvider mqPublisher;

		public FruitHAPService(ISensorRepository sensorRepository, IEnumerable<IController> controllers, IEnumerable<IPlugin> plugins, IMessageQueueProvider mqPublisher, ILogger log)
        {
			this.mqPublisher = mqPublisher;
            this.sensorRepository = sensorRepository;
			this.controllers = controllers;
            this.plugins = plugins;
            this.log = log;
			this.mqPublisher = mqPublisher;
            
        }

        public void Start()
        {
            string mqConnectionString = ConfigurationManager.AppSettings["mqConnectionString"] ?? "";
            string mqPubSubExchangeName = ConfigurationManager.AppSettings["mqPubSubExchangeName"] ?? "FruitHAP_PubSubExchange";
				            
            try
            {
                if (!controllers.Any())
                {
                    log.Error("No controllers loaded. Nothing to do");
                    return;
                }

                sensorRepository.Initialize();

			
                foreach (var controller in controllers)
                {
                    controller.Start();

                }
		   
                try
                {
                    log.Info("Connecting to message queue");
                    mqPublisher.Initialize(mqConnectionString, mqPubSubExchangeName);
                    log.Info("Connected to message queue");	
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("Error initializing message queue. Message: {0}", ex);
                    return;
                }


                log.Info("Initialize plugins");
                if (!plugins.Any())
                {
                    log.Error("No plugins loaded. Nothing to do");
                    return;
                }

                foreach (var plugin in plugins)
                {
                    plugin.Initialize();
                }
                log.Info("Plugins initialized. Summary of status:");

                foreach (var plugin in plugins)
                {
                    log.InfoFormat("Plugin {0}\t{1}", plugin, plugin.IsEnabled ? "ENABLED" : "DISABLED");
                }

                log.InfoFormat("Engine version {0} started",AssemblyHelpers.GetAssemblyVersion(this.GetType().Assembly));

            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error starting service. Message: {0}", ex);
                throw;
            }


        }

        public void Stop()
        {
            log.Info("Stopping plugins..");
            foreach (var plugin in plugins) {
                plugin.Dispose();
            }

            log.Info("Stopping sensors..");
            sensorRepository.Dispose();


            log.Info("Stopping controllers..");
            foreach (var controller in controllers) {
				if (controller.IsStarted) {
					controller.Stop ();
                    controller.Dispose();
				}
			}

			log.Info ("Closing message queue connection");
			if (mqPublisher.IsIntialized) {
				mqPublisher.Dispose ();
			}


        }
    }
}
