using System;
using FruitHAP.Core.Action;
using FruitHAP.Core.SensorRepository;
using Castle.Core.Logging;
using FruitHAP.Core.MQ;
using FruitHap.StandardActions.Messages.Outbound;
using System.Threading.Tasks;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Common.Helpers;
using System.Linq;

namespace FruitHap.StandardActions
{
	public class IncomingRequestAction : IAction
	{
		private readonly ISensorRepository sensorRepository;
		private readonly ILogger logger;
		private readonly IMessageQueueProvider mqProvider;

		public IncomingRequestAction(ISensorRepository sensorRepository, ILogger logger, IMessageQueueProvider publisher)
		{
			this.sensorRepository = sensorRepository;
			this.logger = logger;
			this.mqProvider = publisher;
		}


		#region IAction implementation

		public void Initialize ()
		{
			logger.InfoFormat ("Initializing action {0}", this);
			mqProvider.SubscribeToRequest<SensorMessage, SensorMessage> (HandleIncomingRequest);
		}

		#endregion

		private Task<SensorMessage> HandleIncomingRequest (SensorMessage request)
		{
			Task<SensorMessage> task = 
				new Task<SensorMessage> (() => 
					{
						var result = HandleUnknownRequestType(request);
						logger.DebugFormat("Handling GetValue(). Request = {0}",request);
						if (request == null || string.IsNullOrEmpty(request.SensorName))
						{
							result = new SensorMessage() {TimeStamp = DateTime.Now, Data = "Invalid request", DataType = DataType.ErrorMessage.ToString()};
						}
						else
						{
							if (request.DataType == DataType.GetValue.ToString())
							{
								result = HandleGetValue (request);
							}
							else
							{
								if (request.DataType == DataType.Command.ToString())
								{
									result = HandleCommand (request);
								}
							}
						}

						logger.InfoFormat("Sending message {0}",result);
						return result;
					});
			task.Start ();
			return task;
		}



		SensorMessage HandleCommand (SensorMessage request)
		{
			logger.InfoFormat("Looking for sensor {0}",request.SensorName);
			ISensor sensor = sensorRepository.FindDeviceOfTypeByName<ISensor>(request.SensorName);

			if (sensor == null)
			{
				logger.ErrorFormat("Sensor {0} not found in repository", request.SensorName);
				return new SensorMessage() {TimeStamp = DateTime.Now, SensorName = request.SensorName, Data = "Sensor is not defined", DataType = DataType.ErrorMessage.ToString()};
			}

			logger.InfoFormat("Found sensor: {0}",sensor.Name);

			var requestData = request.Data.ToString ();
			CommandObject command = requestData.ParseJsonString<CommandObject> ();
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

			var callResult = method.Invoke (sensor,null);
					


			return CreateResultMessage(sensor,callResult);
		}

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

		SensorMessage HandleUnknownRequestType (SensorMessage request)
		{
			return new SensorMessage () {
				TimeStamp = DateTime.Now,
				Data = "Unknown data type in request",
				SensorName = request.SensorName,
				SensorType = request.SensorType,
				DataType = DataType.ErrorMessage.ToString ()
			};
		}

		SensorMessage HandleGetValue (SensorMessage request)
		{
			logger.InfoFormat("Looking for sensor {0}",request.SensorName);
			IValueSensor sensor = sensorRepository.FindDeviceOfTypeByName<IValueSensor>(request.SensorName);

			if (sensor == null)
			{
				logger.ErrorFormat("Sensor {0} not found in repository or no support for polling", request.SensorName);
				return new SensorMessage() {TimeStamp = DateTime.Now, SensorName = request.SensorName, Data = "Sensor is not defined or has no support for polling", DataType = DataType.ErrorMessage.ToString()};
			}

			logger.InfoFormat("Found sensor: {0}",sensor.Name);

			return new SensorMessage () {
				TimeStamp = DateTime.Now,
				SensorName = sensor.Name,
				SensorType = sensor.GetTypeString (),
				DataType = DataType.Measurement.ToString (),
				Data = sensor.GetValue()
			};
		}
	}



}

