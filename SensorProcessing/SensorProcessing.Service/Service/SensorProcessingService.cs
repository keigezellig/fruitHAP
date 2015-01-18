using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using SensorProcessing.Common;

namespace SensorProcessing.Service.Service
{
    public class SensorProcessingService : ISensorProcessingService
    {
        private readonly IEnumerable<IBinding> bindings;
        private readonly ILogger log;

        public SensorProcessingService(IEnumerable<IBinding> bindings, ILogger log)
        {
            this.bindings = bindings;
            this.log = log;
        }

        public void Start()
        {
            log.Info("Starting bindings..");

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
