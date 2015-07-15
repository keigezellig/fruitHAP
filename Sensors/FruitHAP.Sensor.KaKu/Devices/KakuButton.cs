using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Common.Helpers;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Sensor.KaKu.Common;
using FruitHAP.Sensor.PacketData.AC;
using FruitHAP.Core.Controller;
using FruitHAP.Core.SensorEventPublisher;

namespace FruitHAP.Sensor.KaKu.Devices
{
	public class KakuButton : KakuDevice, IButton
    {       
		private Command command;
		private ISensorEventPublisher sensorEventPublisher;
	
		public KakuButton(IEventAggregator aggregator, ILogger logger, ISensorEventPublisher sensorEventPublisher) : base(aggregator,logger)
        {
			this.sensorEventPublisher = sensorEventPublisher;            
        }

		public Command Command {
			get {
				return this.command;
			}
			set {
				command = value;
			}
		}
        


		protected override void ProcessReceivedACDataForThisDevice (ACPacket data)
		{
			if (data.Command == command) 
			{				
				sensorEventPublisher.Publish<SensorEvent> (this, null);
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

       
			       

		public override object Clone ()
        {
			return new KakuButton(this.aggregator, this.logger, this.sensorEventPublisher);
        }

		public override string ToString ()
		{
			return string.Format ("[KakuButton: {0}, Command={1}]", base.ToString(), Command);
		}
		

    }
}
