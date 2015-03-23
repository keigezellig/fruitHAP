using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Core.SensorRepository
{
    public class SensorRepository : ISensorRepository
    {
        private readonly ISensorLoader sensorLoader;
        private readonly ILogger logger;
        private IEnumerable<ISensor> sensors;

        public SensorRepository(ISensorLoader sensorLoader, ILogger logger)
        {
            this.sensorLoader = sensorLoader;
            this.logger = logger;
        }

        public void Initialize()
        {
            try
            {
                logger.Info("Loading sensors");
                sensors = sensorLoader.LoadSensors();
				if (sensors.Any())
				{
					logger.InfoFormat("{0} sensors loaded",sensors.Count());
				}
				else
				{
					logger.Warn("No sensors loaded. Check configuration");
				}

            }
            catch (Exception ex)
            {                
                logger.Error("Cannot load sensors",ex);   
            }
            
        }
        
        public IEnumerable<T> FindAllDevicesOfType<T>() where T : ISensor
        {
            return sensors.OfType<T>();
        }

        public T FindDeviceOfTypeByName<T>(string name) where T : ISensor
        {
            return sensors.OfType<T>().SingleOrDefault(f => f.Name == name);
        }


    }
}
