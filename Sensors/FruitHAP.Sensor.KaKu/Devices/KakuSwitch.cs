﻿using System;
using FruitHAP.Core;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using System.Collections.Generic;
using FruitHAP.Common.Helpers;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Sensor.KaKu.Common;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Sensor.PacketData.AC;
using FruitHAP.Core.Controller;
using FruitHAP.Sensor.PacketData.General;
using FruitHAP.Common.EventBus;

namespace FruitHAP.Sensor.KaKu
{
	public class KakuSwitch : KakuDevice, ISwitch
	{
		private Command onCommand;
		private Command offCommand;
		private SwitchState state;
		private Trigger trigger;
		private bool isReadOnly;

		public KakuSwitch(IEventBus eventBus, ILogger logger) : base(eventBus,logger)
		{
		}

		public Command OnCommand {
			get {
				return this.onCommand;
			}
			set {
				onCommand = value;
			}
		}

		public Command OffCommand {
			get {
				return this.offCommand;
			}
			set {
				offCommand = value;
			}
		}
		public Trigger Trigger {
			get {
				return this.trigger;
			}
			set {
				trigger = value;
			}
		}

		public bool IsReadOnly {
			get {
				return isReadOnly;
			}
			set {
				isReadOnly = value;
			}
		}

		public void TurnOn ()
		{
            if (!isReadOnly)
            {
				UpdateState (SwitchState.On);
            }
            else
            {
                logger.Warn("Read only switch!");
            }
		}

		public void TurnOff ()
		{
            if (!isReadOnly)
            {
				UpdateState (SwitchState.Off);
            }
            else
            {
                logger.Warn("Read only switch!");
            }
        }

		private void UpdateState (SwitchState newState)
		{
			if (newState == state) 
			{
				return;
			}

			TriggerControllerEvent(newState);
			logger.Debug ("Waiting for ack...");
			var ack = GetAck ();
			ack.ContinueWith ( (t) => {
				if (t.Result) {
					state = newState;
					TriggerSensorEvent ();
					logger.InfoFormat ("State changed to {0}", state);				
				} else {
					logger.Warn ("Negative or no acknowledgement from controller received, state will not be changed");
				}
			});
		}

		public SwitchState GetState ()
		{
			return state;
		}
        
		public object GetValue ()
		{
			return state;
		}

		#region ICloneable implementation

		public override object Clone ()
		{
			return new KakuSwitch(this.eventBus, this.logger);
		}

		#endregion

		protected override void ProcessReceivedACDataForThisDevice (ACPacket data)
		{
			SwitchState newState = DetermineNewState(data);
			state = newState;
            TriggerSensorEvent();
            logger.InfoFormat("State changed to {0}", state);
        }

		void TriggerSensorEvent ()
		{
			logger.Debug ("Firing sensor event");
			bool fireEvent = true;
			switch (trigger) 
			{
			case Trigger.On:
				fireEvent = (state == SwitchState.On);
				break;
			case Trigger.Off:
				fireEvent = (state == SwitchState.Off);
				break;			
			}

			logger.DebugFormat("Trigger: {2}, New: {0}, FireEvent: {1}",state,fireEvent, trigger);

			if (fireEvent) 
			{
				SensorEventData sensorEvent = new SensorEventData () {
					TimeStamp = DateTime.Now,
					Sender = this,
					EventName = "SensorEvent",
					OptionalData = state
				};

				eventBus.Publish(sensorEvent);
			}
		}

		private void TriggerControllerEvent (SwitchState state)
		{
			logger.Debug ("Firing controller event");
			if (state == SwitchState.Undefined) 
			{
				logger.Error ("Cannot send UNDEFINED switch state to controller");
				return;
			}

			var data = new ACPacket () {
				DeviceId = deviceId,
				UnitCode = unitCode,
				Command = state == SwitchState.On ? OnCommand : OffCommand,
				Level = 0
			};

			eventBus.Publish(new ControllerEventData<ACPacket> () { Direction = Direction.ToController, Payload = data });

		}
			
		private SwitchState DetermineNewState (ACPacket decodedData)
		{
			if (decodedData.Command == onCommand) 
			{
				return SwitchState.On;
			}

			if (decodedData.Command == offCommand) 
			{
				return SwitchState.Off;
			}

			return SwitchState.Undefined;
		}
		public override string ToString ()
		{
			return string.Format ("[KakuSwitch: {0}, OnCommand={1}, OffCommand={2}, Trigger={3}]", base.ToString(), OnCommand, OffCommand,Trigger);
		}
		

	}



}

