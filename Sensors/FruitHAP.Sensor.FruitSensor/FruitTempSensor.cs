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
		private TemperatureValue temperature;

		public ISensorValueType GetValue ()
		{
			return temperature;
		}

		public TemperatureValue Temperature 
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
			temperature = new TemperatureValue ();

			eventBus.Subscribe<ControllerEventData<RFXSensorTemperaturePacket>>(HandleIncomingTempMessage,f => f.Direction == Direction.FromController && f.Payload.SensorId == SensorId);

		}

		public override string ToString ()
		{
			return string.Format ("[FruitTempSensor: Name={1}, Description={2}, Category={3}, SensorId={4}, temperature={0}]", temperature, Name, Description, Category, SensorId);
		}
		

		void HandleIncomingTempMessage (ControllerEventData<RFXSensorTemperaturePacket> obj)
		{
			this.temperature = new TemperatureValue () {
				Value = obj.Payload.TemperatureInCentiCelsius / 100,
				Unit = TemperatureUnit.Celsius
			};


			SensorEventData sensorEvent = new SensorEventData () {
				TimeStamp = DateTime.Now,
				Sender = this,
				EventName = "SensorEvent",
				OptionalData = new OptionalDataContainer(temperature)
			};

			eventBus.Publish(sensorEvent);

		}
	}
}

