using System;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorValueTypes;
using FruitHAP.Common.EventBus;
using Castle.Core.Logging;
using System.Timers;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Common.Configuration;

namespace FruitHAP.Sensor.Fake
{
    public class FakeTemperatureSensor : ITemperatureSensor
    {
        private QuantityValue<TemperatureUnit> temperature;
        private DateTime lastUpdated;

        Timer timer;

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
            return new FakeTemperatureSensor (eventBus, logger);
        }

        #endregion

        #region IDisposable implementation

        public void Dispose ()
        {
            timer.Dispose();
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
        public int Min { get; set; }

        [ConfigurationItem(IsSensorSpecific = true)]
        public int Max { get; set; }

        [ConfigurationItem(IsSensorSpecific = true)]
        public TemperatureUnit Unit { get; set; }

        [ConfigurationItem(IsSensorSpecific = true)]
        public int IntervalInMilliSeconds { get; set; }

        private ILogger logger;
        private IEventBus eventBus;

        public FakeTemperatureSensor(IEventBus eventBus, ILogger logger)
        {
            this.eventBus = eventBus;
            this.logger = logger;
            this.temperature = new QuantityValue<TemperatureUnit> ();
            this.lastUpdated = DateTime.Now;
            this.IntervalInMilliSeconds = 5000;
            this.timer = new Timer(IntervalInMilliSeconds);
            timer.Elapsed += (object sender, ElapsedEventArgs e) => GenerateTemperatureMessage();
            timer.Start();

        }

        public override string ToString()
        {
            return string.Format("[FakeTemperatureSensor: Temperature={0}, Name={1}, Description={2}, Category={3}, Min={4}, Max={5}, Unit={6}]", Temperature, Name, Description, Category, Min, Max, Unit);
        }
        

        public DateTime GetLastUpdateTime ()
        {
            return this.lastUpdated;
        }


        void GenerateTemperatureMessage ()
        {
            var randomizer = new Random();
            var generatedValue = randomizer.Next(Min, Max);
            lastUpdated = DateTime.Now;

            var temperatureValue = new TemperatureQuantity () {
                Value = (double)(generatedValue),
                Unit = this.Unit
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

