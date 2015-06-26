using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using FruitHAP.Core.Action;
using FruitHAP.Core.Sensor;
using System.Configuration;
using System;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.Controller;
using FruitHAP.Core.MQ;

namespace FruitHAP.Core.Service
{
    public class SensorProcessingService : ISensorProcessingService
    {
        private readonly ISensorRepository sensorRepository;
        private readonly IEnumerable<IController> controllers;
        private readonly IEnumerable<IAction> actions;
        private readonly ILogger log;
		private readonly IMessageQueueProvider mqPublisher;

		public SensorProcessingService(ISensorRepository sensorRepository, IEnumerable<IController> controllers, IEnumerable<IAction> actions, IMessageQueueProvider mqPublisher, ILogger log)
        {
			this.mqPublisher = mqPublisher;
            this.sensorRepository = sensorRepository;
			this.controllers = controllers;
            this.actions = actions;
            this.log = log;
			this.mqPublisher = mqPublisher;
            
        }

        public void Start()
        {
			string mqConnectionString = ConfigurationManager.AppSettings ["mqConnectionString"] ?? "";
			string mqPubSubExchangeName = ConfigurationManager.AppSettings ["mqPubSubExchangeName"] ?? "FruitHAP_PubSubExchange";
			string mqRpcExchangeName = ConfigurationManager.AppSettings ["mqRpcExchangeName"] ?? "FruitHAP_RpcExchange";
			string mqRpcQueueName = ConfigurationManager.AppSettings ["mqRpcQueueName"] ?? "FruitHAP_RpcQueue";
				            
			try
			{
			if (!controllers.Any())
			{
				log.Error("No controllers loaded. Nothing to do");
				return;
			}

			log.Info ("Initialize controllers");
			foreach (var controller in controllers)
			{
				controller.Start ();
		
			}

			sensorRepository.Initialize();

		   
			try
			{
				log.Info("Connecting to message queue");
				mqPublisher.Initialize (mqConnectionString,mqPubSubExchangeName,mqRpcExchangeName, mqRpcQueueName);
			}
			catch (Exception ex) 
			{
				log.ErrorFormat ("Error initializing message queue. Message: {0}", ex);
				return;
			}


			if (!actions.Any())
			{
				log.Error("No actions loaded. Nothing to do");
				return;
			}

            log.Info("Initialize actions");
            foreach (var sensorAction in actions)
            {
                sensorAction.Initialize();
            }

			}
			catch (Exception ex) 
			{
				log.ErrorFormat ("Error starting service. Message: {0}", ex);
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
