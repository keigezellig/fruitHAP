using System;
using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using System.Collections.Generic;
using FruitHAP.Common.Helpers;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Sensor.PacketData.AC;
using FruitHAP.Core.Controller;
using FruitHAP.Sensor.PacketData.General;
using System.Threading.Tasks;
using FruitHAP.Common.EventBus;

namespace FruitHAP.Sensor.KaKu.Common
{
	public abstract class KakuDevice : ISensor
	{
		private string name;
		private string description;	
		protected uint deviceId;
		protected byte unitCode;
		protected readonly ILogger logger;
		protected IEventBus eventBus;

		private bool isAckReceived;
		private bool ackValue;

		protected abstract void ProcessReceivedACDataForThisDevice (ACPacket data);

		protected KakuDevice (IEventBus eventBus, ILogger logger)
		{
			this.eventBus = eventBus;
			this.logger = logger;
			eventBus.Subscribe<ControllerEventData<ACPacket>>(HandleIncomingACMessage,f => f.Direction == Direction.FromController && DataReceivedCorrespondsToThisDevice(f.Payload));
			eventBus.Subscribe<ControllerEventData<AckPacket>>(HandleIncomingAckMessage, f => f.Direction == Direction.FromController);
		}
			
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string Description
		{
			get { return description; }
			set { description = value; }
		}

        public string Category { get; set; }

		public uint DeviceId {
			get {
				return this.deviceId;
			}
			set {
				deviceId = value;
			}
		}

		public byte UnitCode {
			get {
				return this.unitCode;
			}
			set {
				unitCode = value;
			}
		}

		public abstract object Clone ();


		void HandleIncomingACMessage (ControllerEventData<ACPacket> obj)
		{
			logger.DebugFormat("Received controller data: {0}", obj.Payload);
			logger.Info("Processing data");
			ProcessReceivedACDataForThisDevice(obj.Payload);
		}

		private bool DataReceivedCorrespondsToThisDevice (ACPacket decodedData)
		{
			return (decodedData.DeviceId == deviceId) && (decodedData.UnitCode == unitCode);
		}

		void HandleIncomingAckMessage (ControllerEventData<AckPacket> obj)
		{			
			isAckReceived = true;
			ackValue = obj.Payload.IsAcknowledged;
		}

		protected async Task<bool> GetAck()
		{
			

			Task<bool> workerTask = new Task<bool>(() => {
				while (!this.isAckReceived)
				{
				}
				this.isAckReceived = false;
				return this.ackValue;
			});

			workerTask.Start();

			return await workerTask.TimeoutAfter(TimeSpan.FromSeconds(5));

		}


		public override string ToString ()
		{
			return string.Format ("Name={0}, Description={1}, DeviceId={2}, UnitCode={3}", Name, Description, DeviceId, UnitCode);
		}

		public void Dispose ()
		{
			eventBus.Unsubscribe<ControllerEventData<ACPacket>> (HandleIncomingACMessage);
			eventBus.Unsubscribe<ControllerEventData<AckPacket>> (HandleIncomingAckMessage);
		}
	}
}

