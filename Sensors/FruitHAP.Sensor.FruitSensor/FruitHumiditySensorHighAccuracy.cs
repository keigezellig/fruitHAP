using System;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Common.EventBus;
using FruitHAP.Core.Controller;
using FruitHAP.Core.Sensor.PacketData.RFXSensor;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHAP.Sensor.FruitSensor
{
    public class FruitHumiditySensorHighAccuracy : IHumiditySensor
	{
        private NumberValue humidity;
		private DateTime lastUpdated;

		public ISensorValueType GetValue ()
		{
            return humidity;
		}

        public NumberValue Humidity
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

		public string Name { get; set; }
		public string Description { get; set;}
        public string DisplayName { get; set; }
		public string Category { get; set; }

		#endregion

		public byte SensorId { get; set; }


		private ILogger logger;
		private IEventBus eventBus;
        private RfxFruitProtocol fruitProtocol;

        public FruitHumiditySensorHighAccuracy(IEventBus eventBus, ILogger logger)
		{
			this.eventBus = eventBus;
			this.logger = logger;	
            this.humidity = new NumberValue();
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
                humidity.Value = (double)(result.Value / 100.0);
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

