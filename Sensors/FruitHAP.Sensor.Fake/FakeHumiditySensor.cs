using System;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorValueTypes;
using FruitHAP.Common.EventBus;
using Castle.Core.Logging;
using System.Timers;
using FruitHAP.Core.Sensor.SensorTypes;

namespace FruitHAP.Sensor.Fake
{
    public class FakeHumiditySensor : IHumiditySensor
    {
        private QuantityValue<String> humidity;
        private DateTime lastUpdated;

        Timer timer;

        public ISensorValueType GetValue ()
        {
            return humidity;
        }

        public QuantityValue<String>  Humidity 
        {
            get 
            {
                return humidity;
            }
        }

        #region ICloneable implementation

        public object Clone ()
        {
            return new FakeHumiditySensor (eventBus, logger);
        }

        #endregion

        #region IDisposable implementation

        public void Dispose ()
        {
            timer.Dispose();
        }

        #endregion

        #region ISensor implementation

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set;}
        public string Category { get; set; }

        #endregion



        private ILogger logger;
        private IEventBus eventBus;

        public FakeHumiditySensor(IEventBus eventBus, ILogger logger)
        {
            this.eventBus = eventBus;
            this.logger = logger;
            this.humidity = new QuantityValue<String> ();
            this.lastUpdated = DateTime.Now;
            this.timer = new Timer(5000);
            timer.Elapsed += (object sender, ElapsedEventArgs e) => GenerateHumidityMessage();
            timer.Start();

        }

        public override string ToString()
        {
            return string.Format("[FakeHumiditySensor: Humidity={0}, Name={1}, Description={2}, Category={3}]", Humidity, Name, Description, Category);
        }
        

        public DateTime GetLastUpdateTime ()
        {
            return this.lastUpdated;
        }


        void GenerateHumidityMessage ()
        {
            var randomizer = new Random();
            var generatedValue = randomizer.Next(0, 100);
            lastUpdated = DateTime.Now;

            var humidityValue = new PercentageQuantity () {
                Value = (double)(generatedValue),
            };

            humidity = new QuantityValue<String> ();
            humidity.Value = humidityValue;

            SensorEventData sensorEvent = new SensorEventData () {
                TimeStamp = lastUpdated,
                Sender = this,
                OptionalData = new OptionalDataContainer(humidity)
            };

            eventBus.Publish(sensorEvent);

        }
    }
}

