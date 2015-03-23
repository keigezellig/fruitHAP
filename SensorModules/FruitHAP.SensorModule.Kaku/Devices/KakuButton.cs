﻿using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.SensorModule.Kaku.Protocol;

namespace FruitHAP.SensorModule.Kaku.Devices
{
	public class KakuButton : IButton, ISensorInitializer, ICloneable
    {
        private readonly IKakuModule module;
        private readonly ILogger logger;
        private string name;
        private string description;
		private KakuProtocolData sensorParameters;


        public KakuButton()
        {
            
        }
        public KakuButton(IKakuModule module, ILogger logger)
        {
            this.module = module;
            this.logger = logger;
        }

        public string Name
        {
            get { return name; }
        }

        public string Description
        {
            get { return description; }
        }

        public event EventHandler ButtonPressed;
        
        public void Initialize(Dictionary<string, string> parameters)
        {            
			sensorParameters = new KakuProtocolData ();

			name = parameters["Name"];
            description = parameters["Description"];
			sensorParameters.DeviceId = Convert.ToUInt32(parameters["DeviceId"],16);
			sensorParameters.UnitCode = Convert.ToByte(parameters["UnitCode"],16);
            sensorParameters.Command = (Command) Enum.Parse(typeof(Command),parameters["Command"]);            
            module.KakuDataReceived += module_KakuDataReceived;
			logger.InfoFormat("Initialized button {0} ({1})",name,sensorParameters);
        }

        void module_KakuDataReceived(object sender, KakuProtocolEventArgs e)
        {
            logger.DebugFormat("Received event: {0}", e.Data);
            if (DataReceivedCorrespondsToTheButton(e.Data))
            {
				logger.Debug ("About to Fire event");
				OnButtonPressed();
            }
        }

		public void PressButton ()
		{
			logger.Debug ("Sending PressButton to module..");
			module.SendData (new KakuProtocolData () {
				DeviceId = sensorParameters.DeviceId,
				Command = sensorParameters.Command,
				UnitCode = sensorParameters.UnitCode,
				Level = 0
			});
		}

        protected virtual void OnButtonPressed()
        {
            if (ButtonPressed != null)
            {
				logger.Debug ("Firing event");
				var localEvent = ButtonPressed;
                localEvent(this, null);
            }
        }

        private bool DataReceivedCorrespondsToTheButton(KakuProtocolData data)
        {
			logger.InfoFormat ("Definition={0} Data={1}", sensorParameters, data);
			return (data.DeviceId == sensorParameters.DeviceId && data.UnitCode == sensorParameters.UnitCode && data.Command == sensorParameters.Command);
        }

        public object Clone()
        {
            return new KakuButton(this.module, this.logger);
        }


    }
}
