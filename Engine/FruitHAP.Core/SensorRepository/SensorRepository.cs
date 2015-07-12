using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor;
using FruitHAP.Common.Helpers;
using FruitHAP.Core.SensorPersister;

namespace FruitHAP.Core.SensorRepository
{
    public class SensorRepository : ISensorRepository
    {
        private readonly ISensorLoader sensorLoader;
        private readonly ILogger logger;
		private IEnumerable<ISensor> sensors;
		ISensorPersister persister;

        public SensorRepository(ISensorLoader sensorLoader, ILogger logger, ISensorPersister persister)
        {
            this.sensorLoader = sensorLoader;
            this.logger = logger;
			this.persister = persister;
        }

        public void Initialize()
        {
            try
            {
                logger.Info("Loading sensors");
				sensors = persister.LoadSensors();

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
        
        public IEnumerable<T> FindAllSensorsOfType<T>() where T : ISensor
        {
            return sensors.OfType<T>();
        }

        public T FindSensorOfTypeByName<T>(string name) where T : ISensor
        {
            return sensors.OfType<T>().SingleOrDefault(f => f.Name == name);
        }

		public IEnumerable<ISensor> GetSensors ()
    	{
			return sensors;
    	}

		public void SaveSensors (IEnumerable<ISensor> sensors)
    	{
			persister.SaveSensors (sensors);			
    	}


    }
}
