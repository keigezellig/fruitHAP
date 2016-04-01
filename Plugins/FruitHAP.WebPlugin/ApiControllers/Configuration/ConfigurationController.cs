using System.Web.Http;
using System.Collections.Generic;
using FruitHAP.Core.SensorPersister;
using System.Linq;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Common.Helpers;
using System;


namespace FruitHAP.Plugins.Web.ApiControllers.Configuration
{
    [RoutePrefix("api/configuration")]
    public class ConfigurationController : ApiController
	{
		private readonly ISensorPersister persister;
		private readonly ISensorRepository repos;

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
		[Route("sensors")]
        [HttpGet]
		public IHttpActionResult GetAllSensors()
		{
			var sensors = GetAllSensorsFromConfiguration ();
            return Ok<IEnumerable<SensorConfigurationItem>> (sensors);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>Returns defintion of a single sensor</remarks>
		/// <param name="name">Name of sensor</param>
		/// <response code="200">A response containing the sensor definition with name {sensorname}</response>
		/// <response code="404">If sensor is not present in the system</response>
        [Route("sensors/{name}", Name = "GetSensorByName")]
        [HttpGet]
		public IHttpActionResult GetSensorByName(string name)
		{
            var sensorConfigItem = GetAllSensorsFromConfiguration().SingleOrDefault(f => f.Name == name);
            if (sensorConfigItem != null)
            {
                return Ok<SensorConfigurationItem>(sensorConfigItem);
            }
            else
            {
                return NotFound ();
            }


  
		}

		private IEnumerable<SensorConfigurationItem> GetAllSensorsFromConfiguration()
		{
            List<SensorConfigurationItem> result = new List<SensorConfigurationItem>();
            var sensorConfig = persister.GetSensorConfiguration();
            foreach (var configItem in sensorConfig)
            {
                string parametersInJson = configItem.Parameters.ToJsonString();
                Dictionary<string, object> parameters = parametersInJson.ParseJsonString<Dictionary<string, object>>();

                SensorConfigurationItem returnItem;
                if (configItem.IsAggegrate)
                {
                    returnItem = CreateAggregateItem(configItem.Type,parameters);
                }
                else
                {
                    returnItem = CreateNonAggregateItem(configItem.Type,parameters);
                }

                result.Add(returnItem);
            }

            return result;
		}

        private SensorConfigurationItem CreateAggregateItem(string type, Dictionary<string, object> parameters)
        {
         
            string name = parameters["Name"].ToString();
            string description = parameters["Description"].ToString();
            string category = parameters["Category"].ToString();
            List<string> inputs = parameters["Inputs"].ToString().ParseJsonString<List<string>>();
            List<string> inputLinks = new List<string>();

            foreach (var input in inputs)
            {
                inputLinks.Add(Url.Link("GetSensorByName", new { name = input}));
            }

            string valueType = repos.GetSensorValueType(name).Name;
            Dictionary<string,string> operations = GetOperations(name);
            return new AggregatedSensor(name, description, category, type, valueType, operations, inputLinks);
        }

        private SensorConfigurationItem CreateNonAggregateItem(string type, Dictionary<string, object> parameters)
        {
            string name = parameters["Name"].ToString();
            string description = parameters["Description"].ToString();
            string category = parameters["Category"].ToString();
            Dictionary<string,object> otherParameters = parameters.Where(f => f.Key != "Name" && f.Key != "Description" && f.Key != "Category").ToDictionary(f => f.Key, f => f.Value);
            string valueType = repos.GetSensorValueType(name).Name;
            Dictionary<string,string> operations = GetOperations(name);
            return new NonAggregatedSensor(name, description, category, type, valueType, operations, otherParameters);
        }

        Dictionary<string, string> GetOperations(string sensorName)
        {
            var operations = repos.GetOperationsForSensor(sensorName);
            return operations.ToDictionary(f => f.Name, g => Url.Link("ExecuteOperation", new { name = sensorName, operation = g.Name}));
        }

        
        

	}
}

