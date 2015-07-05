using System;
using FruitHap.StandardActions.Messages.Outbound;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Common.Helpers;
using System.Linq;
using System.Collections;

namespace FruitHap.StandardActions.IncomingRequest.RequestHandlers
{
	public class CommandHandler : IRequestHandler
	{
		private ILogger logger;
		private ISensorRepository sensorRepository;

		private CommandObject GetCommand (SensorMessage request)
		{
			var requestData = request.Data.ToString ();
			CommandObject command = requestData.ParseJsonString<CommandObject> ();
			return command;
		}


		#region IRequestHandler implementation

		public SensorMessage HandleRequest (SensorMessage request)
		{
			logger.DebugFormat("Handling Command request. Request = {0}",request);
			logger.InfoFormat("Looking for sensor {0}",request.SensorName);
			ISensor sensor = sensorRepository.FindSensorOfTypeByName<ISensor>(request.SensorName);

			if (sensor == null)
			{
				logger.ErrorFormat("Sensor {0} not found in repository", request.SensorName);
				return new SensorMessage() {TimeStamp = DateTime.Now, SensorName = request.SensorName, Data = "Sensor is not defined", DataType = DataType.ErrorMessage.ToString()};
			}

			logger.InfoFormat("Found sensor: {0}",sensor.Name);

			var command = GetCommand (request);

			var type = sensor.GetType ();

			var method = type.GetMethod (command.OperationName);
			if (method == null) 
			{
				return CreateInvalidOperationMessage (sensor, command.OperationName);
			}

			var parameters = method.GetParameters ();
			if (!parameters.All (f => command.Parameters.ContainsKey (f.Name))) 
			{
				return CreateNotAllRequiredParametersAreSpecifiedMessage (sensor, command.OperationName);
			}


			ArrayList arguments = new ArrayList ();
			foreach (var methodParameter in parameters) 
			{				
				var requestParam = command.Parameters.Single (f => f.Key == methodParameter.Name);
				var actualArgument = requestParam.Value;
				try
				{
					var methodArgument = Convert.ChangeType(actualArgument, methodParameter.ParameterType.GetType());
					if (methodArgument == null)
					{
						logger.ErrorFormat ("Argument {0} is not of the correct type for parameter {1} in operation {2} which should be {3} ",actualArgument, methodParameter.Name, command.OperationName, methodParameter.ParameterType.GetType().Name );
						return new SensorMessage () {
							TimeStamp = DateTime.Now,
							SensorName = request.SensorName,
							Data = string.Format("Argument {0} is not of the correct type for parameter {1} in operation {2} which should be {3}",actualArgument, methodParameter.Name, command.OperationName, methodParameter.ParameterType.GetType().Name ),
							DataType = DataType.ErrorMessage.ToString ()
						};
					}

					arguments.Add(methodArgument);
				}
				catch(Exception ex)
				{
					if (ex is FormatException || ex is InvalidCastException || ex is OverflowException) {
						logger.ErrorFormat ("One or more arguments are not in the correct format. Exception: {0}",ex.Message);
						return new SensorMessage () {
							TimeStamp = DateTime.Now,
							SensorName = request.SensorName,
							Data = "One or more arguments are not in the correct format",
							DataType = DataType.ErrorMessage.ToString ()
						};
					} 
					else 
					{
						throw;
					}
				}
			}

			var callResult = method.Invoke (sensor, arguments.ToArray());

			return CreateResultMessage(sensor,callResult);
		}			

		#endregion

	
		SensorMessage CreateResultMessage (ISensor sensor, object callResult)
		{
			return new SensorMessage () {
				TimeStamp = DateTime.Now,
				Data = callResult,
				SensorName = sensor.Name,
				SensorType = sensor.GetTypeString (),
				DataType = DataType.Command.ToString ()
			};
		}

		SensorMessage CreateInvalidOperationMessage (ISensor sensor, string operationName)
		{
			return new SensorMessage () {
				TimeStamp = DateTime.Now,
				Data = string.Format("Operation {0} is not defined",operationName),
				SensorName = sensor.Name,
				SensorType = sensor.GetTypeString(),
				DataType = DataType.ErrorMessage.ToString ()
			};
		}

		SensorMessage CreateNotAllRequiredParametersAreSpecifiedMessage (ISensor sensor, string operationName)
		{
			return new SensorMessage () {
				TimeStamp = DateTime.Now,
				Data = string.Format("Not all required parameters are specified for operation {0} ",operationName),
				SensorName = sensor.Name,
				SensorType = sensor.GetTypeString(),
				DataType = DataType.ErrorMessage.ToString ()
			};
		}

		public CommandHandler (ILogger logger, ISensorRepository sensorRepository)
		{
			this.sensorRepository = sensorRepository;
			this.logger = logger;
		}
	}
}

