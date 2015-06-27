using System;
using FruitHAP.Core.Action;
using FruitHAP.Core.SensorRepository;
using Castle.Core.Logging;
using FruitHAP.Core.MQ;
using FruitHap.StandardActions.Messages.Outbound;
using System.Threading.Tasks;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;

namespace FruitHap.StandardActions
{
	public class GetValueAction : IAction
	{
		private readonly ISensorRepository sensorRepository;
		private readonly ILogger logger;
		private readonly IMessageQueueProvider mqProvider;

		public GetValueAction(ISensorRepository sensorRepository, ILogger logger, IMessageQueueProvider publisher)
		{
			this.sensorRepository = sensorRepository;
			this.logger = logger;
			this.mqProvider = publisher;
		}


		#region IAction implementation

		public void Initialize ()
		{
			mqProvider.SubscribeToRequest<SensorMessage, SensorMessage> (HandleIncomingRequest);
		}

		#endregion

		private Task<SensorMessage> HandleIncomingRequest (SensorMessage request)
		{
			Task<SensorMessage> task = 
				new Task<SensorMessage> (() => 
					{
						logger.DebugFormat("Handling GetValue(). Request = {0}",request);
						if (request == null || string.IsNullOrEmpty(request.SensorName))
						{
							return new SensorMessage() {TimeStamp = DateTime.Now, Data = "Invalid request", DataType = DataType.ErrorMessage.ToString()};
						}

						logger.InfoFormat("Looking for sensor {0}",request.SensorName);
						ISensor sensor = sensorRepository.FindDeviceOfTypeByName<ISensor>(request.SensorName);

						if (sensor == null)
						{
							logger.ErrorFormat("Sensor {0} not found in repository", request.SensorName);
							return new SensorMessage() {TimeStamp = DateTime.Now, SensorName = request.SensorName, Data = "Sensor is not defined", DataType = DataType.ErrorMessage.ToString()};
						}

						logger.InfoFormat("Found sensor: {0}",sensor.Name);
						object value = GetValue(sensor);

						if (value == null)
						{
							logger.ErrorFormat("This sensor has no support for polling");
							return new SensorMessage() {TimeStamp = DateTime.Now, SensorName = sensor.Name, Data = "This sensor has no support for polling"};
						}

						return new SensorMessage() {TimeStamp = DateTime.Now, SensorName = sensor.Name, SensorType = GetSensorType(sensor), DataType = DataType.Measurement.ToString(), Data=value };

					});
			task.Start ();
			return task;
		}

		object GetValue (ISensor sensor)
		{
			if (sensor is ISwitch) 
			{
				return (sensor as ISwitch).GetState ().ToString ();
			}

			if (sensor is ICamera) 
			{
				return (sensor as ICamera).GetImageAsync ().Result;
			}

			return null;

		}

		string GetSensorType (ISensor sensor)
		{
			if (sensor is ISwitch) 
			{
				return "Switch";
			}

			if (sensor is IButton) 
			{
				return "Button";
			}

			if (sensor is ICamera) 
			{
				return "Camera";
			}

			throw new NotSupportedException ("Sensor is of a non supported type");

		}
	}
}

