using System;
using FruitHAP.Core;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Sensor.KaKu.ACProtocol;
using System.Collections.Generic;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Sensor.KaKu
{
	public class KakuSwitch : IReadOnlySwitch, ISensorInitializer, ICloneable
	{
		private IRfxController module;
		private ILogger logger;
		private IACProtocol protocol;
		private string name;
		private string description;
		private UInt32 deviceId;
		private byte unitCode;
		private Command onCommand;
		private Command offCommand;
		private SwitchState state;


		public KakuSwitch(IRfxController module, ILogger logger, IACProtocol protocol)
		{
			this.module = module;
			this.logger = logger;
			this.protocol = protocol;
		}

		#region ISensorInitializer implementation

		public void Initialize (Dictionary<string, string> parameters)
		{
			try
			{								
				name = parameters["Name"];
				logger.InfoFormat("Initializing switch {0}",name);
				description = parameters["Description"];
				deviceId = Convert.ToUInt32(parameters["DeviceId"],16);
				unitCode = Convert.ToByte(parameters["UnitCode"],16);
				onCommand = (Command) Enum.Parse(typeof(Command),parameters["OnCommand"]);
				offCommand = (Command) Enum.Parse(typeof(Command),parameters["OffCommand"]);

				module.ControllerDataReceived += HandleControllerDataReceived;
				this.module.Start ();
				logger.InfoFormat("Initialized switch {0}",name);
			}
			catch (Exception ex) 
			{
				logger.ErrorFormat("Cannot initialize switch {0}. Reason: {1}",name,ex.Message);
			}
		}

		#endregion


		#region IReadOnlySwitch implementation

		public event EventHandler<SwitchEventArgs> StateChanged;

		public SwitchState GetState ()
		{
			return state;
		}

		public string Name
		{
			get { return name; }
		}

		public string Description
		{
			get { return description; }
		}


		#endregion

		#region ICloneable implementation

		public object Clone ()
		{
			return new KakuSwitch(this.module, this.logger, this.protocol);
		}

		#endregion


		private void HandleControllerDataReceived (object sender, ControllerDataEventArgs e)
		{
			logger.DebugFormat("Received controller data: {0}", e.Data.BytesAsString());
			try
			{
				var decodedData = protocol.Decode (e.Data);
				if (DataReceivedCorrespondsToTheSwitch(decodedData))
				{
					SwitchState newState = DetermineNewState(decodedData);
					if (newState != state)
					{						
						state = newState;
						logger.InfoFormat ("State changed to {0}",state);
						OnStateChanged(state);
					}
				}

			}
			catch (ProtocolException ex) 
			{
				logger.ErrorFormat ("Error decoding received data: {0}", ex.Message);
			}
		}

		private bool DataReceivedCorrespondsToTheSwitch (ACProtocolData decodedData)
		{
			return (decodedData.DeviceId == deviceId) && (decodedData.UnitCode == unitCode);
		}

		private SwitchState DetermineNewState (ACProtocolData decodedData)
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

