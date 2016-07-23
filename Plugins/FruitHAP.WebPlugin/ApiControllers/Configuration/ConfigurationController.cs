using System.Web.Http;
using System.Collections.Generic;
using FruitHAP.Core.SensorPersister;
using System.Linq;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Common.Helpers;
using System;
using FruitHAP.Common.Configuration;
using System.Collections;
using Castle.Core.Logging;
using FruitHAP.Plugins.Web.ApiControllers.Configuration.Validators;
using FluentValidation.Results;
using System.Reflection;


namespace FruitHAP.Plugins.Web.ApiControllers.Configuration
{
    [RoutePrefix("api/configuration")]
    public class ConfigurationController : ApiController
	{
		private readonly ISensorPersister persister;
		private readonly ISensorRepository repos;

        private readonly ILogger log;

		public ConfigurationController (ISensorPersister persister, ISensorRepository repos, ILogger log)
		{
			this.repos = repos;
			this.persister = persister;		
            this.log = log;
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
        /// <remarks>Returns the number of sensors defined in the system</remarks>
        /// <response code="200">The number of sensors defined in the system</response>
        [Route("sensors/count")]
        [HttpGet]
        public IHttpActionResult GetSensorCount()
        {
            var count = GetAllSensorsFromConfiguration ().Count();
            return Ok(count);
        }

        [Route("sensors/types")]
        [HttpGet]
        public IHttpActionResult GetSensorTypeNames()
        {
            var types = CollectSensorTypeNames();
            return Ok(types);
        }

        [Route("sensors/types/{name}")]
        [HttpGet]
        public IHttpActionResult GetSensorParameters(string name, bool onlySpecific = false)
        {
            var type = CollectSensorParameters(name,onlySpecific);
            if (type == null)
            {
                return NotFound();
            }

            return Ok(type);
        }

        [Route("sensors/add")]
        [HttpPost]
        public IHttpActionResult AddSensor(SensorUpdateDTO input)
        {            
            SensorUpdateValidator validator = new SensorUpdateValidator();
            var validateResults = validator.Validate(input);
            if (!validateResults.IsValid)
            {
                log.Error("Error in input: ");
                foreach (var error in validateResults.Errors)
                {
                    log.ErrorFormat("{0}: {1}", error.PropertyName, error.ErrorMessage);
                }

                return BadRequest(validateResults.Errors.ToJsonString());
            }
            SensorConfigurationEntry entry = new SensorConfigurationEntry();
            entry.IsAggegrate = false;
            entry.Type = input.Type;
            var parameters = new Dictionary<string,object>();
            parameters["Name"] = input.Name;
            parameters["Description"] = input.Description;
            parameters["DisplayName"] = input.DisplayName;
            parameters["Category"] = input.Category;
            foreach (var specificParameter in input.Parameters)
            {
                if (specificParameter.Type == "String")
                {
                    parameters[specificParameter.Name] = specificParameter.Value;
                }
                else
                {
                    parameters[specificParameter.Name] = Int32.Parse(specificParameter.Value);
                }
            }

            entry.Parameters = parameters;
            persister.AddConfigurationEntry(entry);
            persister.SaveConfiguration();

            return Ok();
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


        private IEnumerable<string> CollectSensorTypeNames()
        {
            var sensorTypes = persister.GetSensorTypes();
            return sensorTypes.Select(t => t.GetType().Name);
        }

        private IEnumerable CollectSensorParameters(string sensorTypeName, bool onlySpecific)
        {
            var sensorType = persister.GetSensorTypes().SingleOrDefault(t => t.GetType().Name == sensorTypeName);
            if (sensorType == null)
            {
                return null;
            }
                
            var props = sensorType.GetConfigurableProperties(onlySpecific);
            //props[3].
            return props.Select(prop =>
                {
                    List<AllowedValueItem> allowedValues = null;
                    var propType = prop.PropertyType;
                    if (propType.IsEnum)
                    {                                             
                        allowedValues = new List<AllowedValueItem>();
                      
                        foreach (var value in Enum.GetValues(propType))
                        {
                            var name = Enum.GetName(propType,value);
                            allowedValues.Add(new AllowedValueItem() {Name = name, Value = (int)value});
                        }
                    }
                    return new SensorParameterItem{Parameter = prop.Name, Type = prop.PropertyType.Name, allowedValues = allowedValues };
                }
            );
        }

        private SensorConfigurationItem CreateAggregateItem(string type, Dictionary<string, object> parameters)
        {
         
            string name = parameters["Name"].ToString();
            string displayName = parameters["DisplayName"].ToString();
            string description = parameters["Description"].ToString();
            string category = parameters["Category"].ToString();
            List<string> inputs = parameters["Inputs"].ToString().ParseJsonString<List<string>>();
            List<string> inputLinks = new List<string>();

            foreach (var input in inputs)
            {
                inputLinks.Add(Url.Route("GetSensorByName", new { name = input}));
            }

            var valueType = repos.GetSensorValueType(name);
            string valueTypeName = null;
            if (valueType != null)
            {
                valueTypeName = valueType.Name;                
            }
            Dictionary<string,string> operations = GetOperations(name);
            return new AggregatedSensor(name, displayName, description, category, type, valueTypeName, operations, inputLinks);
        }

        private SensorConfigurationItem CreateNonAggregateItem(string type, Dictionary<string, object> parameters)
        {
            string name = parameters["Name"].ToString();
            string displayName = parameters["DisplayName"].ToString();
            string description = parameters["Description"].ToString();
            string category = parameters["Category"].ToString();
            Dictionary<string,object> otherParameters = parameters.Where(f => f.Key != "Name" && f.Key != "Description" && f.Key != "DisplayName" && f.Key != "Category").ToDictionary(f => f.Key, f => f.Value);
            var valueType = repos.GetSensorValueType(name);
            string valueTypeName = null;
            if (valueType != null)
            {
                valueTypeName = valueType.Name;                
            }
            Dictionary<string,string> operations = GetOperations(name);
            return new NonAggregatedSensor(name, displayName, description, category, type, valueTypeName, operations, otherParameters);
        }

        Dictionary<string, string> GetOperations(string sensorName)
        {
            var operations = repos.GetOperationsForSensor(sensorName);
            return operations.ToDictionary(f => f.Name, g => Url.Route("ExecuteOperation", new { name = sensorName, operation = g.Name}));

        }

        
        

	}
}

