using System;
using Castle.Core.Logging;
using FruitHAP.Common.Configuration;
using FruitHAP.Common.PhysicalInterfaces;
using System.Reflection;
using System.IO;
using FruitHAP.Core.Sensor;
using System.Linq;
using System.Collections.Generic;
using FruitHAP.Common.Helpers;
using FruitHAP.Controller.Rfx.Configuration;
using FruitHAP.Core.Sensor.Controllers;
using FruitHAP.Sensor.Protocols.ACProtocol;
using Microsoft.Practices.Prism.PubSubEvents;
using Controller.Rfx.ACProtocol;
using FruitHAP.Core.Sensor.Controller;

namespace FruitHAP.Controller.Rfx
{
	public class RfxController : ISensorController
    {
        private readonly IConfigProvider<RfxControllerConfiguration> configProvider;
        private readonly IPhysicalInterfaceFactory physicalInterfaceFactory;
        private readonly ILogger logger;
        private RfxControllerConfiguration configuration;
        private IPhysicalInterface physicalInterface;
		private bool isStarted;
		private static byte SequenceNumber = 1;
		private const string CONFIG_FILENAME = "rfx.json";
		private SubscriptionToken acEventSubscriptionToken;
		private RFXControllerPacketHandlerFactory handlerFactory;
		private List<RfxPacketInfo> supportedPacketTypes;

		private IEventAggregator aggregator;

		public RfxController(IConfigProvider<RfxControllerConfiguration> configProvider, IPhysicalInterfaceFactory physicalInterfaceFactory, ILogger logger, IEventAggregator aggregator)
        {
			this.aggregator = aggregator;
            this.configProvider = configProvider;
            this.physicalInterfaceFactory = physicalInterfaceFactory;
            this.logger = logger;
			this.handlerFactory = new RFXControllerPacketHandlerFactory (logger, aggregator);
			this.supportedPacketTypes = LoadSupportedPacketTypes ();
        }

		private List<RfxPacketInfo> LoadSupportedPacketTypes ()
		{
			
		}

		void HandleIncomingACMessage (ControllerEventData<ACProtocolData> obj)
		{			
			ACProtocol protocol = new ACProtocol (logger);
			byte[] data = protocol.Encode (obj.Payload);
			SendData (data);
		}

        public string Name
        {
            get { return "RFX Controller"; }
        }

		public bool IsStarted
		{
			get
			{
				return isStarted; 
			}
		}

        void PhysicalInterfaceDataReceived(object sender, ExternalDataReceivedEventArgs e)
        {
            
			logger.DebugFormat("Received controller data: {0}", e.Data.BytesAsString());
			try
			{
				IControllerPacketHandler ControllerDataReceivedHandler = handlerFactory.CreateHandler(e.Data);
				ControllerDataReceivedHandler.Handle(e.Data);

			} 
			catch (ProtocolException ex) 
			{
				logger.ErrorFormat ("Error decoding received data: {0}", ex.Message);
			}
        }

       

        public void Start()
        {
			if (!IsStarted) {
				logger.InfoFormat ("Initializing controller {0}", this);

				try {
					acEventSubscriptionToken = aggregator.GetEvent<ACProtocolEvent> ().Subscribe (HandleIncomingACMessage, ThreadOption.PublisherThread, true, f => f.Direction == Direction.ToController);
					configuration = configProvider.LoadConfigFromFile (Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), CONFIG_FILENAME));
					physicalInterface = physicalInterfaceFactory.GetPhysicalInterface (configuration.ConnectionString);
					physicalInterface.DataReceived += PhysicalInterfaceDataReceived;

					physicalInterface.Open ();
					physicalInterface.StartReading ();
					SendResetCommand();
					isStarted = true;
				} catch (Exception ex) {
					isStarted = false;
					throw;
				}
			}

        }

        public void Stop()
        {
            logger.InfoFormat("Stopping module {0}", this);
            physicalInterface.StopReading();
            physicalInterface.Close();
        }

        public void Dispose()
        {
            logger.DebugFormat("Dispose module {0}", this);
			aggregator.GetEvent<ACProtocolEvent> ().Unsubscribe (acEventSubscriptionToken);
            physicalInterface.Dispose();
        }

		

		public void SendData (byte[] data)
		{
			List<byte> dataToBeSend = new List<byte> (data);
			dataToBeSend.Insert (3, SequenceNumber);
			var array = dataToBeSend.ToArray ();
			logger.DebugFormat ("Sending bytes {0} to controller", array.BytesAsString ());
			physicalInterface.Write(array);

			SequenceNumber++;
		}

		public void SendResetCommand()
		{
			logger.Debug ("Sending reset command to controller");
			var dataToBeSend = new byte[] {0x00,0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
			physicalInterface.Write(dataToBeSend);
		}
    }

   
}
