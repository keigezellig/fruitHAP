using System;
using FruitHap.StandardActions.Messages.Outbound;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.SensorRepository;

namespace FruitHap.StandardActions
{
	public class GetValueHandler : IRequestHandler
	{
		private ILogger logger;
		private ISensorRepository sensorRepository;

		#region IRequestHandler implementation

		public SensorMessage HandleRequest (SensorMessage request)
		{
			logger.DebugFormat("Handling GetValue request. Request = {0}",request);
			logger.InfoFormat("Looking for sensor {0}",request.SensorName);
			IValueSensor sensor = sensorRepository.FindSensorOfTypeByName<IValueSensor>(request.SensorName);

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

		#endregion

		public GetValueHandler (ILogger logger, ISensorRepository sensorRepository)
		{
			this.sensorRepository = sensorRepository;
			this.logger = logger;
		}
	}
}

