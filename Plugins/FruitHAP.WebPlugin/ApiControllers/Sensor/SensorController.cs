﻿using System.Web.Http;
using System.Collections.Generic;
using FruitHAP.Core.SensorPersister;
using System.Linq;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.Plugin;
using Castle.Core.Logging;
using System;
using System.Reflection;
using FruitHAP.Core.Sensor.SensorValueTypes;


namespace FruitHAP.Plugins.Web.ApiControllers.Sensor
{
	public class SensorController : ApiController
	{
		private readonly ILogger logger;
		private readonly ISensorRepository sensorRepository;

        public SensorController(ISensorRepository repos, ILogger logger)
		{
			this.sensorRepository = repos;
            this.logger = logger;

		}

	    [Route("api/sensor/getallvalues")]
	    [HttpGet]
	    public IHttpActionResult GetAllValues()
	    {
	        var sensors = sensorRepository.FindAllSensorsOfType<IValueSensor>();
	        var values = sensors.Select(GetValueOfSensor);

	        return Ok<IEnumerable<SensorMessage>>(values);
	    }

	    /// <summary>
		/// 
		/// </summary>
		/// <remarks>Returns the current value of the sensor</remarks>
		/// <param name="name">Name of sensor</param>
        /// <response code="200">Response containing the current value of {sensorname}</response>
		/// <response code="404">If sensor is not present in the system</response>
        /// <response code="400">If sensor is not present in the system</response>
		[Route("api/sensor/{name}")]
        [HttpGet]
		public IHttpActionResult GetValue(string name)
		{
			if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name cannot be empty");

            }

            logger.InfoFormat("Looking for sensor {0}", name);
            IValueSensor sensor = sensorRepository.FindSensorOfTypeByName<IValueSensor>(name);

            
			if (sensor != null) 
            {
                logger.InfoFormat("Found sensor: {0}", sensor.Name);
                return Ok<SensorMessage>(GetValueOfSensor(sensor));
			} 
            else 
			{
                logger.ErrorFormat("Sensor {0} not found in repository or no support for polling", name);
                return NotFound ();
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Calls a function `operation` on a sensor</remarks>
        /// <param name="name">Name of sensor</param>
        /// <param name="operation">Name of the requested operation</param>
        /// <response code="200">Response containing the current value of {sensorname}</response>
        /// <response code="404">If sensor is not present in the system</response>
        /// <response code="400">If function is not present on the sensor</response>
        [Route("api/sensor/{name}/{operation}", Name = "ExecuteOperation")]
        [HttpGet]
        public IHttpActionResult ExecuteOperation(string name, string operation)
        {
			if (operation == "GetValue") 
			{
				return GetValue (name);
			}
			logger.DebugFormat("ExecuteOperation: Sensor = {0}, Operation = {1}", name, operation);
            logger.InfoFormat("Looking for sensor {0}", name);
            ISensor sensor = sensorRepository.FindSensorOfTypeByName<ISensor>(name);

            if (sensor == null)
            {
                logger.ErrorFormat("Sensor {0} not found in repository", name);
                return NotFound();                
            }

            logger.InfoFormat("Found sensor: {0}", sensor.Name);

            var method = sensorRepository.GetOperationForSensor (sensor.Name, operation);

			if (method == null)
            {
                return BadRequest(string.Format("Operation {0} is not available on sensor {1}", operation, name));
            }

            var callResult = method.Invoke(sensor, null);

            return Ok<SensorMessage>(CreateResultMessage(sensor, operation, callResult));
        }

        private SensorMessage GetValueOfSensor(IValueSensor sensor)
        {
			return new SensorMessage()
            {
				TimeStamp = sensor.GetLastUpdateTime(),
                SensorName = sensor.Name,
                Data = new OptionalDataContainer(sensor.GetValue())
            };
        }
        
        private SensorMessage CreateResultMessage(ISensor sensor, string operationName, object callResult)
        {
            return new SensorMessage()
            {
                TimeStamp = DateTime.Now,
                Data = new OptionalDataContainer(new CommandResult() {OperationName = operationName, Result = callResult}),
                SensorName = sensor.Name,
            };
        }


		
	}
}

