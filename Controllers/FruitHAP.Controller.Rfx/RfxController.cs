using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Castle.Core.Logging;
using Controller.Rfx.PacketHandlers;
using FruitHAP.Common.Configuration;
using FruitHAP.Common.Helpers;
using FruitHAP.Common.PhysicalInterfaces;
using FruitHAP.Controller.Rfx.Configuration;
using FruitHAP.Core.Sensor;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Controller;
using FruitHAP.Sensor.PacketData.AC;

namespace FruitHAP.Controller.Rfx
{
	public class RfxController : IController
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
		private RfxControllerPacketHandlerFactory handlerFactory;


		private IEventAggregator aggregator;

		public RfxController(IConfigProvider<RfxControllerConfiguration> configProvider, IPhysicalInterfaceFactory physicalInterfaceFactory, ILogger logger, IEventAggregator aggregator)
        {
			this.aggregator = aggregator;
            this.configProvider = configProvider;
            this.physicalInterfaceFactory = physicalInterfaceFactory;
            this.logger = logger;
			this.handlerFactory = new RfxControllerPacketHandlerFactory (logger, aggregator);

        }

		private List<RfxPacketInfo> LoadPacketTypes (RfxControllerConfiguration configuration)
		{
			
			List<RfxPacketInfo> packetList = new List<RfxPacketInfo> ();
			string logLineTemplate = "{0}..............................................{1}";

			foreach (var packetType in configuration.PacketTypes) {
				foreach (var subPacketType in packetType.SubTypes) {
					RfxPacketType rfxPacketType;

					bool isSupported = RfxPacketType.TryParse (subPacketType.Name, out rfxPacketType);
					bool isEnabled = subPacketType.IsEnabled;

					if (!isSupported || rfxPacketType == RfxPacketType.Unknown) {
						logger.WarnFormat (logLineTemplate, subPacketType.Name, "NOT SUPPORTED");
						continue;
					}

					if (!isEnabled) {
						logger.InfoFormat (logLineTemplate, subPacketType.Name, "DISABLED");
						continue;
					}

					logger.InfoFormat (logLineTemplate, subPacketType.Name, "ENABLED");

					packetList.Add (new RfxPacketInfo () {
						PacketType = rfxPacketType,
						PacketIndicator = packetType.Id,
						SubPacketIndicator = subPacketType.Id,
						LengthByte = packetType.Length
					});

				}
			}


			return packetList;
		}

		void HandleIncomingACMessage (ControllerEventData<ACPacket> obj)
		{			
			RfxACProtocol protocol = new RfxACProtocol (logger);
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
			catch (Exception ex) 
			{
				logger.ErrorFormat ("Error handling received data: {0}", ex.Message);
			}
        }

       

        public void Start()
        {
			if (!IsStarted) {
				logger.InfoFormat ("Initializing controller {0}", this);

				try {
					acEventSubscriptionToken = aggregator.GetEvent<ACPacketEvent> ().Subscribe (HandleIncomingACMessage, ThreadOption.PublisherThread, true, f => f.Direction == Direction.ToController);
					configuration = configProvider.LoadConfigFromFile (Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), CONFIG_FILENAME));
					var packetTypes = LoadPacketTypes(configuration);
					handlerFactory.Initialize(packetTypes);
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
			aggregator.GetEvent<ACPacketEvent> ().Unsubscribe (acEventSubscriptionToken);
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
