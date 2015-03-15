using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using FruitHAP.Core.Action;
using FruitHAP.Core.Sensor;
using System.Configuration;
using System;

namespace FruitHAP.Core.Service
{
    public class SensorProcessingService : ISensorProcessingService
    {
        private readonly ISensorRepository sensorRepository;
        private readonly IEnumerable<ISensorModule> modules;
        private readonly IEnumerable<IAction> actions;
        private readonly ILogger log;
		private readonly IMessageQueueProvider mqPublisher;

		public SensorProcessingService(ISensorRepository sensorRepository, IEnumerable<ISensorModule> modules, IEnumerable<IAction> actions, IMessageQueueProvider mqPublisher, ILogger log)
        {
			this.mqPublisher = mqPublisher;
            this.sensorRepository = sensorRepository;
            this.modules = modules;
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




			if (!modules.Any())
            {
                log.Error("No modules loaded. Nothing to do");
                return;
            }

				            
            sensorRepository.Initialize();

            log.Info("Starting modules");

            foreach (var module in modules)
            {
                module.Start();
            }

            if (!actions.Any())
            {
                log.Error("No actions loaded. Nothing to do");
                return;
            }

			try
			{
				mqPublisher.Initialize (mqConnectionString,mqPubSubExchangeName,mqRpcExchangeName, mqRpcQueueName);
			}
			catch (Exception ex) 
			{
				log.ErrorFormat ("Error initializing message queue. Message: {0}", ex);
				return;
			}

            log.Info("Initialize actions");
            foreach (var sensorAction in actions)
            {
                sensorAction.Initialize();
            }


			try
			{
				mqPublisher.Initialize (mqConnectionString,mqExchangeName);
			}
			catch (Exception ex) 
			{
				log.ErrorFormat ("Error initializing message queue. Message: {0}", ex.Message);
				return;
			}

        }

        public void Stop()
        {

			log.Info("Stopping modules..");
			foreach (var module in modules) {
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
