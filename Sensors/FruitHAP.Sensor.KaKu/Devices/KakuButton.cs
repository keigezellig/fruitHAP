using System;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Sensor.KaKu.Common;
using FruitHAP.Sensor.PacketData.AC;
using FruitHAP.Core.Controller;
using FruitHAP.Common.EventBus;
using FruitHAP.Core;
using FruitHAP.Core.Sensor;

namespace FruitHAP.Sensor.KaKu.Devices
{
	public class KakuButton : KakuDevice, IButton
    {       
		private ACCommand command;
	
		public KakuButton(IEventBus eventBus, ILogger logger) : base(eventBus,logger)
        {
        }

		public ACCommand Command {
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
				SensorEventData sensorEvent = new SensorEventData () {
					TimeStamp = DateTime.Now,
					Sender = this,
					OptionalData = null
				};

				eventBus.Publish(sensorEvent);
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

			eventBus.Publish(new ControllerEventData<ACPacket> () { Direction = Direction.ToController, Payload = data });

		}

		public override object Clone ()
        {
			return new KakuButton(this.eventBus, this.logger);
        }

		public override string ToString ()
		{
			return string.Format ("[KakuButton: {0}, Command={1}]", base.ToString(), Command);
		}
		

    }
}
