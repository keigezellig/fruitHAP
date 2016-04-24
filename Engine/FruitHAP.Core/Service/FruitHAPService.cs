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
                log.Info("Plugins initialized");

            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error starting service. Message: {0}", ex);
                throw;
            }


        }

        public void Stop()
        {
			log.Info("Stopping modules..");
			foreach (var module in controllers) {
				if (module.IsStarted) {
					module.Stop ();
				}
			}

			log.Info ("Closing message queue connection");
			if (mqPublisher.IsIntialized) {
				mqPublisher.Dispose ();
			}
        }
    }
}
