using System;
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
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHAP.Sensor.KaKu
{
	public class KakuSwitch : KakuDevice, ISwitch
	{
		private Command onCommand;
		private Command offCommand;
		private OnOffValue state;
		private Trigger trigger;
		private bool isReadOnly;
		private DateTime lastUpdateTime;

		public KakuSwitch(IEventBus eventBus, ILogger logger) : base(eventBus,logger)
		{
			state = new OnOffValue () { Value = StateValue.Undefined };
			this.lastUpdateTime = DateTime.Now;
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
				UpdateState (StateValue.On);
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
				UpdateState (StateValue.Off);
            }
            else
            {
                logger.Warn("Read only switch!");
            }
        }

		private void UpdateState (StateValue newState)
		{
			if (state.Value == newState) 
			{
				return;
			}

			TriggerControllerEvent(newState);
			logger.Debug ("Waiting for ack...");
			try
			{
			var ack = GetAck ().Result;
			if (ack) {
					state.Value = newState;
					lastUpdateTime = DateTime.Now;
				TriggerSensorEvent ();
				logger.InfoFormat ("State changed to {0}", state);				
			} else {
				logger.Warn ("Negative or no acknowledgement from controller received, state will not be changed");
			}
			}
			catch (AggregateException aex) 
			{
				aex.Handle (ex => 
					{
						if (ex is TimeoutException)
						{
							logger.Warn ("Time out occured while waiting on ack, state will be set to undefined");
							state.Value = StateValue.Undefined;
							lastUpdateTime = DateTime.Now;
						}
						return ex is TimeoutException;
					});
				
			}

			/*ack.ContinueWith ( (t) => {
				if (t.Result) {
					state = newState;
					TriggerSensorEvent ();
					logger.InfoFormat ("State changed to {0}", state);				
				} else {
					logger.Warn ("Negative or no acknowledgement from controller received, state will not be changed");
				}
			});*/
		}

		public OnOffValue State 
		{
			get 
			{
				return state;
			}
		}
        
		public ISensorValueType GetValue ()
		{
			return state;
		}

		public DateTime GetLastUpdateTime ()
		{
			return lastUpdateTime;
		}

		#region ICloneable implementation

		public override object Clone ()
		{
			return new KakuSwitch(this.eventBus, this.logger);
		}

		#endregion

		protected override void ProcessReceivedACDataForThisDevice (ACPacket data)
		{
			StateValue newState = DetermineNewState(data);
			state.Value = newState;
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
				fireEvent = (state.Value == StateValue.On);
				break;
			case Trigger.Off:
				fireEvent = (state.Value == StateValue.Off);
				break;			
			}

			logger.DebugFormat("Trigger: {2}, New: {0}, FireEvent: {1}",state,fireEvent, trigger);

			if (fireEvent) 
			{
				SensorEventData sensorEvent = new SensorEventData () {
					TimeStamp = lastUpdateTime,
					Sender = this,
					EventName = "SensorEvent",
					OptionalData = new OptionalDataContainer(state)
				};

				eventBus.Publish(sensorEvent);
			}
		}

		private void TriggerControllerEvent (StateValue state)
		{
			logger.Debug ("Firing controller event");
			if (state == StateValue.Undefined) 
			{
				logger.Error ("Cannot send UNDEFINED switch state to controller");
				return;
			}

			var data = new ACPacket () {
				DeviceId = deviceId,
				UnitCode = unitCode,
				Command = state == StateValue.On ? OnCommand : OffCommand,
				Level = 0
			};

			eventBus.Publish(new ControllerEventData<ACPacket> () { Direction = Direction.ToController, Payload = data });

		}
			
		private StateValue DetermineNewState (ACPacket decodedData)
		{
			if (decodedData.Command == onCommand) 
			{
				return StateValue.On;
			}

			if (decodedData.Command == offCommand) 
			{
				return StateValue.Off;
			}

			return StateValue.Undefined;
		}
		public override string ToString ()
		{
			return string.Format ("[KakuSwitch: {0}, OnCommand={1}, OffCommand={2}, Trigger={3}]", base.ToString(), OnCommand, OffCommand,Trigger);
		}
		

	}



}

