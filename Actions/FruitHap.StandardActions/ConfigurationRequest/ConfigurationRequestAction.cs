using Castle.Core.Logging;
using FruitHAP.Core.Action;
using FruitHAP.Core.MQ;
using FruitHAP.Core.SensorPersister;
using FruitHAP.Core.SensorRepository;
using System;
using System.Collections.Generic;

namespace FruitHap.StandardActions.ConfigurationRequest
{
	public class ConfigurationRequestAction : RpcAction<ConfigurationMessage, ConfigurationMessage>
	{
        private ISensorPersister sensorPersister;
        private ISensorRepository sensorRepository;
        

        public ConfigurationRequestAction(ISensorRepository sensorRepository, ISensorPersister sensorPersister, ILogger logger, IMessageQueueProvider publisher) : base(logger,publisher)
		{
            this.sensorRepository = sensorRepository;
            this.sensorPersister = sensorPersister;
		}

        protected override ConfigurationMessage ProcessRequest(ConfigurationMessage request)
        {
            ConfigurationMessage responseMessage = new ConfigurationMessage()
            {
                TimeStamp = DateTime.Now,
                OperationName = request.OperationName
            };
            
            if (request.OperationName == "GetAllSensors")
            {
                IEnumerable<SensorConfigurationEntry> sensorData = GetAllSensors();
                responseMessage.Data = sensorData;
                responseMessage.MessageType = ConfigurationMessageType.Response;
            }

            if (request.OperationName == "GetAllSensorsByType")
            {

                IEnumerable<SensorConfigurationEntry> sensorData = GetAllSensors();
                responseMessage.Data = sensorData;
                responseMessage.MessageType = ConfigurationMessageType.Response;
            }


        }

        private IEnumerable<SensorConfigurationEntry> GetAllSensors()
        {
            var sensorList = sensorRepository.GetSensors();
            var sensorData = sensorPersister.GetSensorConfiguration(sensorList);
            return sensorData;
        }

        private IEnumerable<SensorConfigurationEntry> GetAllSensorsByType(string typeName)
        {
            var sensorList = sensorRepository.FindAllSensorsOfTypeByTypeName(typeName);
            var sensorData = sensorPersister.GetSensorConfiguration(sensorList);
            return sensorData;
        }

    }

    

    


}

