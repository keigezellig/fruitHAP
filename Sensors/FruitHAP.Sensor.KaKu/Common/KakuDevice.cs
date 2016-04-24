using FruitHAP.Core.Sensor;
using Castle.Core.Logging;
using FruitHAP.Sensor.PacketData.AC;
using FruitHAP.Core.Controller;
using FruitHAP.Common.EventBus;
using FruitHAP.Core.Sensor.PacketData.General;

namespace FruitHAP.Sensor.KaKu.Common
{
	public abstract class KakuDevice: ISensor
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

        protected virtual void ProcessNakPacket(NakPacket<ControllerEventData<ACPacket>> payload)
        {            
        }



		protected KakuDevice (IEventBus eventBus, ILogger logger)
		{
			this.eventBus = eventBus;
			this.logger = logger;
			eventBus.Subscribe<ControllerEventData<ACPacket>>(HandleIncomingACMessage,f => f.Direction == Direction.FromController && DataReceivedCorrespondsToThisDevice(f.Payload));			
            eventBus.Subscribe<NakPacket<ControllerEventData<ACPacket>>>(HandleNakMessage, filter => filter.Data.Payload.DeviceId == this.deviceId && filter.Data.Payload.UnitCode == this.unitCode );
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

        void HandleNakMessage(NakPacket<ControllerEventData<ACPacket>> obj)
        {
            logger.DebugFormat("{0}: NAK received for message {1}. Reason: {2}", this.Name, obj.Data.Payload, obj.Reason);
            ProcessNakPacket(obj);
        }


		public override string ToString ()
		{
			return string.Format ("Name={0}, Description={1}, DeviceId={2}, UnitCode={3}", Name, Description, DeviceId, UnitCode);
		}

		public void Dispose ()
		{
			eventBus.Unsubscribe<ControllerEventData<ACPacket>> (HandleIncomingACMessage);
           // eventBus.Unsubscribe<ControllerEventData<NakPacket<ControllerEventData<ACPacket>>>>(HandleNakPacket);			
		}
	}
}

