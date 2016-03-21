using System.Web.Http;
using System.Collections.Generic;
using FruitHAP.Core.SensorPersister;
using System.Linq;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.SensorRepository;


namespace FruitHap.Web.ApiControllers.Configuration
{
	public class ConfigurationController : ApiController
	{
		readonly ISensorPersister persister;
		readonly ISensorRepository repos;

		public ConfigurationController (ISensorPersister persister, ISensorRepository repos)
		{
			this.repos = repos;
			this.persister = persister;			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>Returns all sensors defined in the system</remarks>
		/// <response code="200">A response containing all the sensors defined in the system</response>
		[Route("api/configuration/sensors")]
		public IHttpActionResult Get()
		{
			var sensors = GetAllSensors ();
			return Ok<IEnumerable<SensorConfigurationEntry>> (sensors);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>Returns defintion of a single sensor</remarks>
		/// <param name="name">Name of sensor</param>
		/// <response code="200">A response containing the sensor definition with name {sensorname}</response>
		/// <response code="404">If sensor is not present in the system</response>
		[Route("api/configuration/sensors/{name}")]
		public IHttpActionResult Get(string name)
		{
			ISensor sensor = repos.GetSensors ().SingleOrDefault (f => f.Name == name);
			if (sensor != null) {
				var sensorData = persister.GetSensorConfiguration(new List<ISensor>() {sensor});
				return Ok<object> (sensorData.ElementAt(0));
			} else 
			{
				return NotFound ();
			}
		}

		private IEnumerable<SensorConfigurationEntry> GetAllSensors()
		{
			var sensorList = repos.GetSensors();
			var sensorData = persister.GetSensorConfiguration(sensorList);
			return sensorData;
		}

	}
}

