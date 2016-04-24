using System;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Common.EventBus;
using FruitHAP.Core.Controller;
using FruitHAP.Core.Sensor.PacketData.RFXSensor;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHAP.Sensor.FruitSensor.FruitTempSensor
{
	public class FruitTempSensor : ITemperatureSensor
	{
		private QuantityValue<TemperatureUnit> temperature;
		private DateTime lastUpdated;

		public ISensorValueType GetValue ()
		{
			return temperature;
		}

		public QuantityValue<TemperatureUnit>  Temperature 
		{
			get 
			{
				return temperature;
			}
		}

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
			this.temperature = new QuantityValue<TemperatureUnit> ();
			this.lastUpdated = DateTime.Now;

			eventBus.Subscribe<ControllerEventData<RFXSensorTemperaturePacket>>(HandleIncomingTempMessage,f => f.Direction == Direction.FromController && f.Payload.SensorId == SensorId);

		}

		public override string ToString ()
		{
			return string.Format ("[FruitTempSensor: Name={1}, Description={2}, Category={3}, SensorId={4}, temperature={0}]", temperature, Name, Description, Category, SensorId);
		}

		public DateTime GetLastUpdateTime ()
		{
			return this.lastUpdated;
		}
		

		void HandleIncomingTempMessage (ControllerEventData<RFXSensorTemperaturePacket> obj)
		{
			lastUpdated = DateTime.Now;
			var temperatureValue = new TemperatureQuantity () {
				Value = obj.Payload.TemperatureInCentiCelsius / 100,
				Unit = TemperatureUnit.Celsius
			};
			temperature = new QuantityValue<TemperatureUnit> ();
			temperature.Value = temperatureValue;


			SensorEventData sensorEvent = new SensorEventData () {
				TimeStamp = lastUpdated,
				Sender = this,
				OptionalData = new OptionalDataContainer(temperature)
			};

			eventBus.Publish(sensorEvent);

		}
	}
}

