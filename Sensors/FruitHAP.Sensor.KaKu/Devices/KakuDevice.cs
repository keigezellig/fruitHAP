using System;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using System.Collections.Generic;
using FruitHAP.Common.Helpers;
using FruitHAP.Core.Sensor.Controllers;
using FruitHAP.Sensor.Protocols.ACProtocol;

namespace FruitHAP.Sensor.KaKu
{
	public abstract class KakuDevice : ISensor, ISensorInitializer, ICloneable
	{
		private string name;
		private string description;	
		protected uint deviceId;
		protected byte unitCode;
		protected readonly IACController controller;
		protected readonly ILogger logger;


		protected abstract void InitializeSpecificDevice (Dictionary<string, string> parameters);
		protected abstract void ProcessReceivedACDataForThisDevice (ACProtocolData data);

		protected KakuDevice (IACController controller, ILogger logger)
		{
			this.logger = logger;
			this.controller = controller;
		}

		#region ISensorInitializer implementation

		public void Initialize (Dictionary<string, string> parameters)
		{
			try
			{								
				name = parameters["Name"];
				description = parameters["Description"];
				deviceId = Convert.ToUInt32(parameters["DeviceId"],16);
				unitCode = Convert.ToByte(parameters["UnitCode"],16);
				InitializeSpecificDevice(parameters);
			
				controller.ACDataReceived += HandleControllerDataReceived;
				this.controller.Start ();
				logger.InfoFormat("Initialized KaKu device {0}",name);
			}
			catch (Exception ex) 
			{
				logger.ErrorFormat("Cannot initialize device {0}. Reason: {1}",name,ex.Message);
			}
		}
		#endregion

		public string Name
		{
			get { return name; }
		}

		public string Description
		{
			get { return description; }
		}

		public abstract object Clone ();

		private void HandleControllerDataReceived (object sender, ACProtocolEventArgs e)
		{
			logger.DebugFormat("Received controller data: {0}", e.Data);
			try
			{
				if (DataReceivedCorrespondsToThisDevice(e.Data))
				{
					logger.Info("Processing data");
					ProcessReceivedACDataForThisDevice(e.Data);
				}
				else
				{
					logger.Info("Not for me!");
				}
			}
			catch (ProtocolException ex) 
			{
				logger.ErrorFormat ("Error decoding received data: {0}", ex.Message);
			}
		}

		private bool DataReceivedCorrespondsToThisDevice (ACProtocolData decodedData)
		{
			return (decodedData.DeviceId == deviceId) && (decodedData.UnitCode == unitCode);
		}


	}
}

