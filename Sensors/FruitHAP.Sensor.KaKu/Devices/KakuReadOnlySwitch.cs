using System;
using Castle.Core.Logging;
using FruitHAP.Common.EventBus;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Core.Sensor.SensorValueTypes;
using FruitHAP.Sensor.KaKu.Common;
using FruitHAP.Sensor.PacketData.AC;

namespace FruitHAP.Sensor.KaKu
{
	public class KakuReadOnlySwitch : KakuDevice, ISwitch
	{
		private ACCommand onCommand;
		private ACCommand offCommand;
		private OnOffValue state;
		private Trigger trigger;		
		private DateTime lastUpdateTime;

        public KakuReadOnlySwitch(IEventBus eventBus, ILogger logger) : base(eventBus,logger)
		{
			state = new OnOffValue () { Value = StateValue.Undefined };
			this.lastUpdateTime = DateTime.Now;
		}

		public ACCommand OnCommand {
			get {
				return this.onCommand;
			}
			set {
				onCommand = value;
			}
		}

		public ACCommand OffCommand {
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
			return new KakuReadOnlySwitch(this.eventBus, this.logger);
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
					OptionalData = new OptionalDataContainer(state)
				};

				eventBus.Publish(sensorEvent);
			}
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

