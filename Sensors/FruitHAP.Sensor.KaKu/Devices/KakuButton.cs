using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Sensor.KaKu.ACProtocol;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Sensor.KaKu.Devices
{
	public class KakuButton : KakuDevice, IButton
    {       
		private Command command;
	
		public KakuButton(IRfxController controller, ILogger logger, IACProtocol protocol) : base(controller,logger,protocol)
        {            
        }


        protected override void InitializeSpecificDevice (Dictionary<string, string> parameters)
		{

			command = (Command)Enum.Parse (typeof(Command), parameters ["Command"]);
		}


		public event EventHandler ButtonPressed;

		protected override void ProcessReceivedACDataForThisDevice (ACProtocolData data)
		{
			if (data.Command == command) 
			{
				logger.Debug ("About to Fire event");
				OnButtonPressed();
			}
		}
			        

		public void PressButton ()
		{
			logger.Debug ("Sending PressButton to module..");
			var encodedData = protocol.Encode (new ACProtocolData () {
				DeviceId = deviceId,
				UnitCode = unitCode,
				Command = command,
				Level = 0
			});

			controller.SendData (encodedData);
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
			       

		public override object Clone ()
        {
			return new KakuButton(this.controller, this.logger, this.protocol);
        }


    }
}
