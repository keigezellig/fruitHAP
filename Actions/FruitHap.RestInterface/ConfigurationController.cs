using System;
using System.Web.Http;
using System.Collections.Generic;
using FruitHAP.Core.SensorPersister;
using System.Linq;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Core.Action;
using FruitHap.Core.Action;
using FruitHAP.Core.SensorRepository;

namespace FruitHap.RestInterface
{
	public class ConfigurationController : ApiController
	{
		ISensorPersister persister;

		ISensorRepository repos;

		public ConfigurationController (ISensorPersister persister, ISensorRepository repos)
		{
			this.repos = repos;
			this.persister = persister;
			
		}

		[Route("api/configuration/sensors")]
		public IHttpActionResult Get ()
		{
			var sensors = GetAllSensors ();
			return Ok<IEnumerable<SensorConfigurationEntry>> (sensors);
		}

		[Route("api/configuration/sensors/{name}")]
		public IHttpActionResult Get(string name)
		{
			var sensor = repos.GetSensors ().SingleOrDefault (f => f.Name == name);
			if (sensor != null) {
				return Ok<object> (sensor);
			} else 
			{
				return NotFound ();
			}
		}

		[Route("api/configuration/sensors/{name}/{operation}")]
		public IHttpActionResult Get(string name, string operation)
		{
			var sensor = repos.GetSensors ().SingleOrDefault (f => f.Name == name);
			if (sensor != null) 
			{
				if (operation == "TurnOn") 
				{
					(sensor as ISwitch).TurnOn();
					var result = CreateResultMessage (sensor, operation, null);
					return Ok (result);
				}
				if (operation == "TurnOff") 
				{
					(sensor as ISwitch).TurnOff();
					var result = CreateResultMessage (sensor, operation, null);
					return Ok (result);
				}
				if (operation == "GetValue") 
				{
					var result = new SensorMessage () {
						TimeStamp = DateTime.Now,
						SensorName = sensor.Name,
						EventType = "GetValue",
						Data = new OptionalDataContainer((sensor as IValueSensor).GetValue())
					};

					return Ok (result);
				}
				return Ok ();
			} else 
			{
				return NotFound ();
			}
		}


		SensorMessage CreateResultMessage (ISensor sensor, string operationName, object callResult)
		{
			return new SensorMessage () {
				TimeStamp = DateTime.Now,
				Data = new {OperationName = operationName, Result = callResult},
				SensorName = sensor.Name,
				EventType = "Command"
			};
		}


	

		/*if (request.OperationName == "GetAllSensors")
            {
                IEnumerable<SensorConfigurationEntry> sensorData = GetAllSensors();
                responseMessage.Data = sensorData;
                responseMessage.MessageType = ConfigurationMessageType.Response;
                return responseMessage;
            }*/

		private IEnumerable<SensorConfigurationEntry> GetAllSensors()
		{
			var sensorList = repos.GetSensors();
			var sensorData = persister.GetSensorConfiguration(sensorList);
			return sensorData;
		}
	}
}

