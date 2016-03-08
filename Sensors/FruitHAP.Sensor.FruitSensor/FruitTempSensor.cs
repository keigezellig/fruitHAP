using System;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Common.EventBus;
using FruitHAP.Core.Controller;
using FruitHAP.Core.Sensor.PacketData.RFXSensor;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;

namespace FruitHAP.Sensor.FruitSensor.FruitTempSensor
{
	public class FruitTempSensor : ITemperatureSensor
	{

		private double temperature;
		private TemperatureUnit unit;

		#region ITemperatureSensor implementation

		public double GetTemperature ()
		{
			return temperature;
		}

		public TemperatureUnit GetUnit ()
		{
			return unit;
		}

		#endregion

		#region IValueSensor implementation

		public object GetValue ()
		{
			return new {Temperature = temperature, Unit = unit};
		}

		#endregion

		#region ICloneable implementation

		public object Clone ()
		{
			return new FruitTempSensor (eventBus, logger);
		}

		#endregion

		#region IDisposable implementation

		public void Dispose ()
		{
			eventBus.Unsubscribe<ControllerEventData<RFXSensorTemperaturePacket>> (HandleIncomingTempMessage);
		}

		#endregion

		#region ISensor implementation

		public string Name { get; set; }
		public string Description { get; set;}
		public string Category { get; set; }

		#endregion

		public byte SensorId { get; set; }


		private ILogger logger;
		private IEventBus eventBus;

		public FruitTempSensor(IEventBus eventBus, ILogger logger)
		{
			this.eventBus = eventBus;
			this.logger = logger;

			eventBus.Subscribe<ControllerEventData<RFXSensorTemperaturePacket>>(HandleIncomingTempMessage,f => f.Direction == Direction.FromController && f.Payload.SensorId == SensorId);

		}

		public override string ToString ()
		{
			return string.Format ("[FruitTempSensor: temperature={0}, unit={1}, Name={2}, Description={3}, Category={4}, SensorId={5}]", temperature, unit, Name, Description, Category, SensorId);
		}
		

		void HandleIncomingTempMessage (ControllerEventData<RFXSensorTemperaturePacket> obj)
		{
			this.temperature = obj.Payload.TemperatureInCentiCelsius / 100;
			this.unit = TemperatureUnit.Celsius;

			SensorEventData sensorEvent = new SensorEventData () {
				TimeStamp = DateTime.Now,
				Sender = this,
				EventName = "SensorEvent",
				OptionalData = GetValue()
			};

			eventBus.Publish(sensorEvent);

		}
	}
}

