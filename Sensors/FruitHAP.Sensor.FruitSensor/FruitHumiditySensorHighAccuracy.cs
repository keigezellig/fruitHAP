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
    public class FruitHumiditySensorHighAccuracy : IHumiditySensor
	{
        private QuantityValue<String> humidity;
		private DateTime lastUpdated;

		public ISensorValueType GetValue ()
		{
            return humidity;
		}

        public QuantityValue<String> Humidity
		{
			get 
			{
                return humidity;
			}
		}

		#region ICloneable implementation

		public object Clone ()
		{
            return new FruitHumiditySensorHighAccuracy (eventBus, logger);
		}

		#endregion

		#region IDisposable implementation

		public void Dispose ()
		{
			eventBus.Unsubscribe<ControllerEventData<RFXMeterPacket>> (HandleIncomingMessage);
		}

		#endregion

		#region ISensor implementation

        [ConfigurationItem]
        public string Name { get; set; }
        [ConfigurationItem]
        public string Description { get; set;}
        [ConfigurationItem]
        public string DisplayName { get; set; }
        [ConfigurationItem]
        public string Category { get; set; }

		#endregion
        [ConfigurationItem(IsSensorSpecific = true)]
		public byte SensorId { get; set; }


		private ILogger logger;
		private IEventBus eventBus;
        private RfxFruitProtocol fruitProtocol;

        public FruitHumiditySensorHighAccuracy(IEventBus eventBus, ILogger logger)
		{
			this.eventBus = eventBus;
			this.logger = logger;	
            this.humidity = new QuantityValue<String> ();
			this.lastUpdated = DateTime.Now;
            this.fruitProtocol = new RfxFruitProtocol();
            eventBus.Subscribe<ControllerEventData<RFXMeterPacket>>(HandleIncomingMessage,f => f.Direction == Direction.FromController && f.Payload.SensorId == SensorId);

		}

		public override string ToString ()
		{
            return string.Format ("[FruitHumiditySensorHighAccuracy: Name={1}, Description={2}, Category={3}, SensorId={4}, Humidity={0}]", Humidity, Name, Description, Category, SensorId);
		}

		public DateTime GetLastUpdateTime ()
		{
			return this.lastUpdated;
		}
		

        void HandleIncomingMessage (ControllerEventData<RFXMeterPacket> obj)
		{
			lastUpdated = DateTime.Now;
            var result = fruitProtocol.Decode(obj.Payload.Value);
            if (result.Quantity == RfxFruitQuantity.HumidityInPercentage)
            {
                var humidityValue = new PercentageQuantity () {
                    Value = (double)(result.Value / 100.0)
                };

                humidity = new QuantityValue<String> ();
                humidity.Value = humidityValue;

                SensorEventData sensorEvent = new SensorEventData()
                {
                    TimeStamp = lastUpdated,
                    Sender = this,
                    OptionalData = new OptionalDataContainer(humidity)
                };

                eventBus.Publish(sensorEvent);
            }

		}
	}
}

