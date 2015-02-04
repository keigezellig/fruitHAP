using System;
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
        private uint deviceId;
        private byte unitCode;
        private Command command;


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
            name = parameters["Name"];
            description = parameters["Description"];
            deviceId = UInt32.Parse(parameters["DeviceId"]);
            unitCode = byte.Parse(parameters["UnitCode"]);
            command = (Command) Enum.Parse(typeof(Command),parameters["Command"]);            
            module.KakuDataReceived += module_KakuDataReceived;
            logger.InfoFormat("Initialized button {0}, {1}, {2:X} ({2}), {3:x}, {4}", name, description, deviceId, unitCode, command);
        }

        void module_KakuDataReceived(object sender, KakuProtocolEventArgs e)
        {
            logger.DebugFormat("Received event: {0}", e.Data);
            if (DataReceivedCorrespondsToTheButton(e.Data))
            {
                OnButtonPressed();
            }
        }

        protected virtual void OnButtonPressed()
        {
            if (ButtonPressed != null)
            {
                var localEvent = ButtonPressed;
                localEvent(this, null);
            }
        }

        private bool DataReceivedCorrespondsToTheButton(KakuProtocolData data)
        {
            return (data.DeviceId == deviceId && data.UnitCode == unitCode && data.Command == command);
        }

        public object Clone()
        {
            return new KakuButton(this.module, this.logger);
        }
    }
}
