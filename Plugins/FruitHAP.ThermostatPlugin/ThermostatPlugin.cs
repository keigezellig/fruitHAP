using System;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Core.Plugin;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.MQ;
using FruitHAP.Common.Configuration;
using System.IO;
using System.Reflection;
using FruitHAP.Common.EventBus;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Core.Sensor.SensorValueTypes;
using FruitHAP.Plugins.Thermostat.Configuration;
using System.Threading;


namespace FruitHAP.Plugins.Thermostat
{
	public class ThermostatPlugin : IPlugin
	{
		private readonly ISensorRepository sensorRepository;
		private readonly ILogger logger;
		private readonly IMessageQueueProvider mqProvider;
        private IConfigProvider<ThermostatConfiguration> configurationProvider;
        private const string CONFIG_FILENAME = "thermostat.json";
        private ThermostatConfiguration configuration;
		private IEventBus eventBus;

		ISwitch switchAbove;
		ISwitch switchBelow;

		public ThermostatPlugin(ISensorRepository sensorRepository, 
								  ILogger logger, 
                                  IConfigProvider<ThermostatConfiguration> configurationProvider, 
								  IMessageQueueProvider mqProvider,
								  IEventBus eventBus)
		{
			this.eventBus = eventBus;
			this.sensorRepository = sensorRepository;
			this.logger = logger;
			this.mqProvider = mqProvider;
			this.configurationProvider = configurationProvider;
		}


		public void Initialize()
		{
			
			logger.InfoFormat ("Initializing plugin {0}", this);
			logger.InfoFormat ("Loading configuration");
			configuration = configurationProvider.LoadConfigFromFile (Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), CONFIG_FILENAME));

			switchAbove = sensorRepository.FindSensorOfTypeByName<ISwitch> (configuration.SwitchAbove);
			switchBelow = sensorRepository.FindSensorOfTypeByName<ISwitch> (configuration.SwitchBelow);
			if (switchAbove == null && switchBelow == null) 
			{
                logger.ErrorFormat ("Cannot find switches {0} and {1}, so plugin will not be started ",configuration.SwitchAbove, configuration.SwitchBelow);
                return;
			}

			switchAbove.TurnOff ();
			switchBelow.TurnOff ();
			eventBus.Subscribe<SensorEventData> (HandleSensorEvent, f => f.Sender.Name.Contains (configuration.TemperatureSensor) && f.Sender is ITemperatureSensor);
		}


		void UnSubscribe ()
		{			
			eventBus.Unsubscribe<SensorEventData> (HandleSensorEvent);
		}

		void HandleSensorEvent (SensorEventData data)
		{						
			SensorMessage sensorMessage = new SensorMessage ();
            var tempValue = data.OptionalData.Content as QuantityValue<TemperatureUnit>;
            if (tempValue.Value.Value > configuration.ThresholdHot) 
			{
				logger.Info ("Temperature above upper limit");
				if (switchBelow.State.Value  != StateValue.Off ) 
				{
					switchBelow.TurnOff ();
				}
                Thread.Sleep(500);
				if (switchAbove.State.Value != StateValue.On) {
					switchAbove.TurnOn ();
				}

				sensorMessage = new SensorMessage () 
				{
					TimeStamp = DateTime.Now,
					SensorName = data.Sender.Name,
					Data = CreateResponse(data,true),
					EventType = data.EventName
				};
			} 
            else if (tempValue.Value.Value < configuration.ThresholdCold) 
			{
				logger.Info ("Temperature below lower limit");
				if (switchAbove.State.Value != StateValue.Off) 
				{
					switchAbove.TurnOff ();
				}
                Thread.Sleep(500);

				if (switchBelow.State.Value != StateValue.On) {
					switchBelow.TurnOn ();
				}

				sensorMessage = new SensorMessage () 
				{
					TimeStamp = DateTime.Now,
					SensorName = data.Sender.Name,
					Data = CreateResponse(data,false),
					EventType = data.EventName
				};

			} 
			else 
			{
				switchAbove.TurnOff ();
                Thread.Sleep(500);
				switchBelow.TurnOff ();
			}

		
			mqProvider.Publish (sensorMessage, configuration.RoutingKey);

		}




		private ThermostatResponse CreateResponse (SensorEventData data, bool isAbove)
		{
			if (isAbove) {
				return new ThermostatResponse () {
					NotificationText = configuration.NotificationTextAbove,
					Priority = NotificationPriority.High,
					OptionalData = data.OptionalData
				};
			} else
				return new ThermostatResponse () {
					NotificationText = configuration.NotificationTextBelow,
					Priority = NotificationPriority.High,
					OptionalData = data.OptionalData
				};
		}



        public void Dispose()
        {
            logger.DebugFormat("Dispose action {0}", this);
            UnSubscribe();
		}
	}




}

