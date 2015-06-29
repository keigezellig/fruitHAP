﻿using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Common.Helpers;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Sensor.KaKu.Common;
using FruitHAP.Sensor.PacketData.AC;
using FruitHAP.Core.Controller;

namespace FruitHAP.Sensor.KaKu.Devices
{
	public class KakuButton : KakuDevice, IButton
    {       
		private Command command;
	
		public KakuButton(IEventAggregator aggregator, ILogger logger) : base(aggregator,logger)
        {            
        }


        protected override void InitializeSpecificDevice (Dictionary<string, string> parameters)
		{
			command = (Command)Enum.Parse (typeof(Command), parameters ["Command"]);
		}


		public event EventHandler ButtonPressed;

		protected override void ProcessReceivedACDataForThisDevice (ACPacket data)
		{
			if (data.Command == command) 
			{
				logger.Debug ("About to Fire event");
				OnButtonPressed();
			}
		}
			        

		public void PressButton ()
		{
			logger.Debug ("Sending PressButton to controller..");
			var data = new ACPacket () {
				DeviceId = deviceId,
				UnitCode = unitCode,
				Command = command,
				Level = 0
			};

			aggregator.GetEvent<ACPacketEvent> ().Publish (new ControllerEventData<ACPacket> () { Direction = Direction.ToController, Payload = data });

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
			return new KakuButton(this.aggregator, this.logger);
        }


    }
}
