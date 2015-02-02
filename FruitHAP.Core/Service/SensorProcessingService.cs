using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using FruitHAP.Core.Action;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.Service
{
    public class SensorProcessingService : ISensorProcessingService
    {
        private readonly ISensorRepository sensorRepository;
        private readonly IEnumerable<ISensorModule> modules;
        private readonly IEnumerable<IAction> actions;
        private readonly ILogger log;

        public SensorProcessingService(ISensorRepository sensorRepository, IEnumerable<ISensorModule> modules, IEnumerable<IAction> actions, ILogger log)
        {
            this.sensorRepository = sensorRepository;
            this.modules = modules;
            this.actions = actions;
            this.log = log;
            
        }

        public void Start()
        {
            if (!modules.Any())
            {
                log.Error("No modules loaded. Nothing to do");
                return;
            }

            log.Info("Loading sensors");
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
