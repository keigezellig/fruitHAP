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
using FruitHAP.Controller.Rfx.InternalPacketData;

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
		private SubscriptionToken setModeEventSubscriptionToken;
		private RfxControllerPacketHandlerFactory handlerFactory;

		List<RfxPacketInfo> packetTypes;

		byte usedSequenceNumber;

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
						LengthByte = packetType.Length,
						ProtocolReceiverSensitivityFlag = (ProtocolReceiverSensitivityFlags)Enum.Parse(typeof(ProtocolReceiverSensitivityFlags),subPacketType.SensitivityFlag)
					});

				}
			}

			RfxPacketInfo interfacePacket = GetInterfacePacket ();
			packetList.Add (interfacePacket);

			return packetList;
		}

		private ProtocolReceiverSensitivityFlags GetSensitivityFlags (List<RfxPacketInfo> packetTypes )
		{
			ProtocolReceiverSensitivityFlags result = ProtocolReceiverSensitivityFlags.Off;

			foreach (var packetType in packetTypes) 
			{
				if (packetType.PacketType != RfxPacketType.Interface) 
				{
					result |= packetType.ProtocolReceiverSensitivityFlag;
				}
			}

			return result;
		}

		private RfxPacketInfo GetInterfacePacket ()
		{
			return new RfxPacketInfo () {
				LengthByte = 0x0D,
				PacketType = RfxPacketType.Interface,
				PacketIndicator = 0x01,
				SubPacketIndicator = 0x00
			};
		}

		void HandleIncomingACMessage (ControllerEventData<ACPacket> obj)
		{			
			RfxACProtocol protocol = new RfxACProtocol (logger);
			byte[] data = protocol.Encode (obj.Payload);
			SendData (data);
		}

		void HandleIncomingSetModeResponse (ControllerEventData<StatusPacket> obj)
		{
			var responsePacket = obj.Payload;
			if (responsePacket.SequenceNumber == this.usedSequenceNumber) 
			{
				logger.Debug ("Received mode command response from controller");
				foreach (var packetType in this.packetTypes) {
					if (!responsePacket.ReceiverSensitivity.HasFlag (packetType.ProtocolReceiverSensitivityFlag)) {
						logger.WarnFormat ("Controller is not enabled to receive packets of type {0}. These packets will be ignored", packetType.PacketType);
					}
				}
			}

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
				IControllerPacketHandler controllerPacketdHandler = handlerFactory.CreateHandler(e.Data);
				if (controllerPacketdHandler == null)
				{
					logger.Warn("Ignoring received data. Controller can't handle this.");
				}
				controllerPacketdHandler.Handle(e.Data);

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
					setModeEventSubscriptionToken = aggregator.GetEvent<SetModeResponsePacketEvent> ().Subscribe (HandleIncomingSetModeResponse, ThreadOption.PublisherThread, true, f => f.Direction == Direction.FromController);
					configuration = configProvider.LoadConfigFromFile (Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), CONFIG_FILENAME));
					packetTypes = LoadPacketTypes(configuration);
					handlerFactory.Initialize(packetTypes);
					physicalInterface = physicalInterfaceFactory.GetPhysicalInterface (configuration.ConnectionString);
					physicalInterface.DataReceived += PhysicalInterfaceDataReceived;
					physicalInterface.Open ();
					physicalInterface.StartReading ();
					SendResetCommand();
					ProtocolReceiverSensitivityFlags sensitivityFlags = GetSensitivityFlags(packetTypes);
					SendModeCommand(sensitivityFlags);
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
			aggregator.GetEvent<SetModeResponsePacketEvent> ().Unsubscribe (setModeEventSubscriptionToken);
            physicalInterface.Dispose();
        }

		

		public void SendData (byte[] data)
		{			
			List<byte> bytesToBeSend = new List<byte> (data);
			bytesToBeSend.Insert (0, (byte)data.Count ());
			bytesToBeSend[3] = SequenceNumber;
			logger.DebugFormat ("Sending bytes {0} to controller", data.BytesAsString ());
			physicalInterface.Write(bytesToBeSend.ToArray());
			usedSequenceNumber = SequenceNumber;
			SequenceNumber++;
		}

		//0D 00 00 00 00 00 00 00 00 00 00 00 00 00 
		public void SendResetCommand()
		{
			logger.Debug ("Sending reset command to controller");
			var dataToBeSend = new byte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
			physicalInterface.Write(dataToBeSend);
		}

		//0D 00 00 SEQ 03 53 00 SB1 SB2 SB3 00 00 00 00
		private void SendModeCommand(ProtocolReceiverSensitivityFlags protocolReceiverSensitivity)
		{
			logger.Debug ("Sending mode command to controller");
			byte[] sensitivityBytes = GetSensitivityBytes (protocolReceiverSensitivity);
			List<byte> dataToBeSend = new List<byte> ();
			dataToBeSend.AddRange(new byte[] {0x00,0x00,0xFF,0x03,0x53,0x00});
			dataToBeSend.AddRange (sensitivityBytes);
			dataToBeSend.AddRange (new byte[] { 0x00, 0x00, 0x00, 0x00 });
			SendData (dataToBeSend.ToArray());
		}


		byte[] GetSensitivityBytes (ProtocolReceiverSensitivityFlags protocolReceiverSensitivityFlags)
		{
			uint intvalue = (uint) protocolReceiverSensitivityFlags;
			return BitConverter.GetBytes(intvalue).Reverse().Skip(1).ToArray();
		}
    }

   
	[Flags]
	public enum ProtocolReceiverSensitivityFlags
	{
		Off = 0x00,
		X10 = 0x01,
		ARC = 0x02,
		AC = 0x04,
		HomeEasyEU = 0x08,
		MeianTech = 0x10,
		OregonScientific = 0x20,
		AtiRemote = 0x40,
		Visonic = 0x80,
		Mertik = 0x100,
		ADLightwaveRF = 0x200,
		HidekiUPM = 0x400,
		LaCrosse = 0x800,
		FS20 = 0x1000,
		ProGuard = 0x2000,
		BlindT0 = 0x4000,
		BlindT1T2T3T4 = 0x8000,
		AEBlyss = 0x10000,
		Rubicson = 0x20000,
		FineOffsetViking = 0x40000,
		Lighting4 = 0x80000,
		RSL2Revolt = 0x100000,
		ByronSX = 0x200000,
		RFU = 0x400000,
		Undecoded= 0x800000		
	}

}
