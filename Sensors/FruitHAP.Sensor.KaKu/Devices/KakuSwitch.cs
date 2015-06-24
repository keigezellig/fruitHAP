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

namespace FruitHAP.Sensor.KaKu
{
	public class KakuSwitch : KakuDevice, IReadOnlySwitch
	{
		private Command onCommand;
		private Command offCommand;
		private SwitchState state;


		public KakuSwitch(IEventAggregator aggregator, ILogger logger) : base(aggregator,logger)
		{
		}

		#region ISensorInitializer implementation

		protected override void InitializeSpecificDevice (Dictionary<string, string> parameters)
		{
			onCommand = (Command) Enum.Parse(typeof(Command),parameters["OnCommand"]);
			offCommand = (Command) Enum.Parse(typeof(Command),parameters["OffCommand"]);
		}

		#endregion


		#region IReadOnlySwitch implementation

		public event EventHandler<SwitchEventArgs> StateChanged;

		public SwitchState GetState ()
		{
			return state;
		}

		#endregion

		#region ICloneable implementation

		public override object Clone ()
		{
			return new KakuSwitch(this.aggregator, this.logger);
		}

		#endregion

		protected override void ProcessReceivedACDataForThisDevice (ACPacket data)
		{
			SwitchState newState = DetermineNewState(data);
			if (newState != state)
			{						
				state = newState;
				logger.InfoFormat ("State changed to {0}",state);
				OnStateChanged(state);
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

		protected virtual void OnStateChanged(SwitchState newState)
		{

			if (StateChanged != null) 
			{
				var localEvent = StateChanged;
				localEvent.Invoke (this, new SwitchEventArgs () { NewState = newState });
			}
		}


	}
}

