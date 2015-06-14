using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Sensor.KaKu.ACProtocol;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Sensor.KaKu.Devices
{
	public class KakuButton : IButton, ISensorInitializer, ICloneable
    {
        private readonly IRfxController module;
        private readonly ILogger logger;
		private readonly IACProtocol protocol;
        private string name;
        private string description;
        private ACProtocolData sensorDefinition;


        public KakuButton()
        {
            
        }
        public KakuButton(IRfxController module, ILogger logger, IACProtocol protocol)
        {
            this.module = module;
            this.logger = logger;
			this.protocol = protocol;
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
			try
			{
				logger.InfoFormat("Initializing button {0} ({1})",name,sensorDefinition);
				sensorDefinition = new ACProtocolData ();
				name = parameters["Name"];
	            description = parameters["Description"];
				sensorDefinition.DeviceId = Convert.ToUInt32(parameters["DeviceId"],16);
				sensorDefinition.UnitCode = Convert.ToByte(parameters["UnitCode"],16);
	            sensorDefinition.Command = (Command) Enum.Parse(typeof(Command),parameters["Command"]);

				module.ControllerDataReceived += HandleControllerDataReceived;
				this.module.Start ();
				logger.InfoFormat("Initialized button {0} ({1})",name,sensorDefinition);
			}
			catch (Exception ex) 
			{
				logger.ErrorFormat("Cannot initialize button {0} ({1}). Reason: {2}",name,sensorDefinition,ex.Message);
			}

        }

        void HandleControllerDataReceived (object sender, ControllerDataEventArgs e)
        {
			logger.DebugFormat("Received controller data: {0}", e.Data.BytesAsString());
			try
			{
				var decodedData = protocol.Decode (e.Data);
				if (DataReceivedCorrespondsToTheButton(decodedData))
				{
					logger.Debug ("About to Fire event");
					OnButtonPressed();
				}

			}
			catch (ProtocolException ex) 
			{
				logger.ErrorFormat ("Error decoding received data: {0}", ex.Message);
			}

        }

        

		public void PressButton ()
		{
			logger.Debug ("Sending PressButton to module..");
			var encodedData = protocol.Encode (new ACProtocolData () {
				DeviceId = sensorDefinition.DeviceId,
				Command = sensorDefinition.Command,
				UnitCode = sensorDefinition.UnitCode,
				Level = 0
			});

			module.SendData (encodedData);
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

        private bool DataReceivedCorrespondsToTheButton(ACProtocolData data)
        {
			return (data.DeviceId == sensorDefinition.DeviceId && data.UnitCode == sensorDefinition.UnitCode && data.Command == sensorDefinition.Command);
        }

        public object Clone()
        {
			return new KakuButton(this.module, this.logger, this.protocol);
        }


    }
}
