using System;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using System.Collections.Generic;
using FruitHAP.Common.Helpers;
using FruitHAP.Core.Sensor.Controllers;
using FruitHAP.Sensor.Protocols.ACProtocol;
using Microsoft.Practices.Prism.PubSubEvents;

namespace FruitHAP.Sensor.KaKu
{
	public abstract class KakuDevice : ISensor, ISensorInitializer, ICloneable
	{
		private string name;
		private string description;	
		protected uint deviceId;
		protected byte unitCode;
		protected readonly ILogger logger;
		protected IEventAggregator aggregator;

		protected abstract void InitializeSpecificDevice (Dictionary<string, string> parameters);
		protected abstract void ProcessReceivedACDataForThisDevice (ACProtocolData data);



		protected KakuDevice (IEventAggregator aggregator, ILogger logger)
		{
			this.aggregator = aggregator;
			this.logger = logger;

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
							
				aggregator.GetEvent<ACProtocolEvent> ().Subscribe (HandleIncomingACMessage, ThreadOption.PublisherThread, false, f => f.Direction == Direction.FromController && DataReceivedCorrespondsToThisDevice(f.Payload));
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


		void HandleIncomingACMessage (ControllerEventData<ACProtocolData> obj)
		{
			logger.DebugFormat("Received controller data: {0}", obj.Payload);
			logger.Info("Processing data");
			ProcessReceivedACDataForThisDevice(obj.Payload);
		}

		private bool DataReceivedCorrespondsToThisDevice (ACProtocolData decodedData)
		{
			return (decodedData.DeviceId == deviceId) && (decodedData.UnitCode == unitCode);
		}


	}
}

