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
using FruitHAP.Core.SensorEventPublisher;

namespace FruitHAP.Sensor.KaKu
{
	public class KakuSwitch : KakuDevice, ISwitch
	{
		private Command onCommand;
		private Command offCommand;
		private SwitchState state;
		private Trigger trigger;
		private ISensorEventPublisher sensorEventPublisher;

		public KakuSwitch(IEventAggregator aggregator, ILogger logger, ISensorEventPublisher sensorEventPublisher) : base(aggregator,logger)
		{
			this.sensorEventPublisher = sensorEventPublisher;
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
			return new KakuSwitch(this.aggregator, this.logger, this.sensorEventPublisher);
		}

		#endregion

		protected override void ProcessReceivedACDataForThisDevice (ACPacket data)
		{
			SwitchState newState = DetermineNewState(data);
			if (newState != state)
			{										
				state = newState;
				logger.InfoFormat ("State changed to {0}",state);

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
					sensorEventPublisher.Publish<SensorEvent> (this, state.ToString ());
				}

			}

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

