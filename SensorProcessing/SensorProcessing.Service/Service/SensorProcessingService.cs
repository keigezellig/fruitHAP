using System.Collections.Generic;
using Castle.Core.Logging;
using SensorProcessing.Common;

namespace SensorProcessing.Service.Service
{
    public class SensorProcessingService : ISensorProcessingService
    {
        private readonly IEnumerable<IBinding> bindings;
        private readonly IEnumerable<ISensorAction> actions;
        private readonly IDeviceRepository deviceRepository;
        private readonly ILogger log;

        public SensorProcessingService(IEnumerable<IBinding> bindings, IEnumerable<ISensorAction> actions, IDeviceRepository deviceRepository, ILogger log)
        {
            this.bindings = bindings;
            this.actions = actions;
            this.deviceRepository = deviceRepository;
            this.log = log;
        }

        public void Start()
        {
            log.Info("Loading devices");
            deviceRepository.LoadDevices();
            
            log.Info("Initialize actions");
            foreach (var sensorAction in actions)
            {
                sensorAction.Initialize();
            }

            log.Info("Starting bindings");

            foreach (var binding in bindings)
            {
                binding.Start();
            }

        }

        public void Stop()
        {
            log.Info("Stopping bindings..");
            foreach (var binding in bindings)
            {
                binding.Stop();
               
            }
        }
    }
}
