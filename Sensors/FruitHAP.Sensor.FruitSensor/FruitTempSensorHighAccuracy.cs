using System;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Common.EventBus;
using FruitHAP.Core.Controller;
using FruitHAP.Core.Sensor.PacketData.RFXSensor;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor.SensorValueTypes;
using FruitHAP.Common.Configuration;

namespace FruitHAP.Sensor.FruitSensor
{
    public class FruitTempSensorHighAccuracy : ITemperatureSensor
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
            return new FruitTempSensorHighAccuracy (eventBus, logger);
		}

		#endregion

		#region IDisposable implementation

		public void Dispose ()
		{
			eventBus.Unsubscribe<ControllerEventData<RFXMeterPacket>> (HandleIncomingTempMessage);
		}

		#endregion

		#region ISensor implementation

        [ConfigurationItem]
		public string Name { get; set; }
        [ConfigurationItem]
        public string DisplayName { get; set; }
        [ConfigurationItem]
        public string Description { get; set;}
        [ConfigurationItem]
        public string Category { get; set; }

		#endregion

        [ConfigurationItem(IsSensorSpecific = true)]
        public byte SensorId { get; set; }


		private ILogger logger;
		private IEventBus eventBus;
        private RfxFruitProtocol fruitProtocol;

        public FruitTempSensorHighAccuracy(IEventBus eventBus, ILogger logger)
		{
			this.eventBus = eventBus;
			this.logger = logger;
			this.temperature = new QuantityValue<TemperatureUnit> ();
			this.lastUpdated = DateTime.Now;
            this.fruitProtocol = new RfxFruitProtocol();

            eventBus.Subscribe<ControllerEventData<RFXMeterPacket>>(HandleIncomingTempMessage,f => f.Direction == Direction.FromController && f.Payload.SensorId == SensorId);

		}

		public override string ToString ()
		{
			return string.Format ("[FruitTempSensorHighAccuracy: Name={1}, Description={2}, Category={3}, SensorId={4}, temperature={0}]", temperature, Name, Description, Category, SensorId);
		}

		public DateTime GetLastUpdateTime ()
		{
			return this.lastUpdated;
		}
		

        void HandleIncomingTempMessage (ControllerEventData<RFXMeterPacket> obj)
		{
			lastUpdated = DateTime.Now;
            var result = fruitProtocol.Decode(obj.Payload.Value);
            if (result.Quantity == RfxFruitQuantity.TemperatureInCentiCelsius)
            {
                var temperatureValue = new TemperatureQuantity()
                {
                    Value = (double)(result.Value / 100.0),
                    Unit = TemperatureUnit.Celsius
                };

                temperature = new QuantityValue<TemperatureUnit>();
                temperature.Value = temperatureValue;

                SensorEventData sensorEvent = new SensorEventData()
                {
                    TimeStamp = lastUpdated,
                    Sender = this,
                    OptionalData = new OptionalDataContainer(temperature)
                };

                eventBus.Publish(sensorEvent);
            }

		}
	}
}

