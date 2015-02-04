using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using FruitHAP.Core.Action;
using FruitHAP.Core.Sensor;
using System.Configuration;

namespace FruitHAP.Core.Service
{
    public class SensorProcessingService : ISensorProcessingService
    {
        private readonly ISensorRepository sensorRepository;
        private readonly IEnumerable<ISensorModule> modules;
        private readonly IEnumerable<IAction> actions;
        private readonly ILogger log;
		private readonly IMessageQueuePublisher mqPublisher;

		public SensorProcessingService(ISensorRepository sensorRepository, IEnumerable<ISensorModule> modules, IEnumerable<IAction> actions, IMessageQueuePublisher mqPublisher, ILogger log)
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
			mqPublisher.Initialize (mqConnectionString);

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


            log.Info("Initialize actions");
            foreach (var sensorAction in actions)
            {
                sensorAction.Initialize();
            }

        }

        public void Stop()
        {
            log.Info("Stopping modules..");
            foreach (var module in modules)
            {
                module.Stop();
            }
        }
    }
}
