﻿using System;
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
        
		private const string CONFIG_FILENAME = "rfx.json";

		private readonly IConfigProvider<RfxControllerConfiguration> configProvider;
        private readonly IPhysicalInterfaceFactory physicalInterfaceFactory;
        private readonly ILogger logger;
        private RfxControllerConfiguration configuration;
		private bool isStarted;
		private SubscriptionToken acEventSubscriptionToken;
		private SubscriptionToken setModeEventSubscriptionToken;
		private RfxControllerPacketHandlerFactory handlerFactory;
		private List<RfxPacketInfo> packetTypes;
		private RfxDevice rfxDevice;
		private IEventAggregator aggregator;

		public RfxController(IConfigProvider<RfxControllerConfiguration> configProvider, IPhysicalInterfaceFactory physicalInterfaceFactory, ILogger logger, IEventAggregator aggregator)
        {
			this.aggregator = aggregator;
            this.configProvider = configProvider;
            this.physicalInterfaceFactory = physicalInterfaceFactory;
            this.logger = logger;
			this.handlerFactory = new RfxControllerPacketHandlerFactory (logger, aggregator);
			this.rfxDevice = new RfxDevice (logger);
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

        private void RfxDataReceived(object sender, ExternalDataReceivedEventArgs e)
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
					SetSubscriptionTokens ();
					LoadConfiguration ();
					OpenRfxDevice ();
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
			rfxDevice.Close ();
        }

        public void Dispose()
        {
            logger.DebugFormat("Dispose module {0}", this);
			aggregator.GetEvent<ACPacketEvent> ().Unsubscribe (acEventSubscriptionToken);
			aggregator.GetEvent<SetModeResponsePacketEvent> ().Unsubscribe (setModeEventSubscriptionToken);
			rfxDevice.Dispose();

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

		private void HandleIncomingACMessage (ControllerEventData<ACPacket> obj)
		{			
			RfxACProtocol protocol = new RfxACProtocol (logger);
			byte[] data = protocol.Encode (obj.Payload);
			rfxDevice.SendData(data);
		}

		private void HandleIncomingSetModeResponse (ControllerEventData<StatusPacket> obj)
		{
			var responsePacket = obj.Payload;
			if (responsePacket.SequenceNumber == rfxDevice.PreviousSequenceNumber) 
			{
				logger.Debug ("Received mode command response from controller");
				foreach (var packetType in this.packetTypes) {
					if (!responsePacket.ReceiverSensitivity.HasFlag (packetType.ProtocolReceiverSensitivityFlag)) {
						logger.WarnFormat ("Controller is not enabled to receive packets of type {0}. These packets will be ignored", packetType.PacketType);
					}
				}
			}
		}


		private void LoadConfiguration ()
		{
			configuration = configProvider.LoadConfigFromFile (Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), CONFIG_FILENAME));
			packetTypes = LoadPacketTypes (configuration);
			handlerFactory.Initialize (packetTypes);
		}

		private void SetSubscriptionTokens ()
		{
			acEventSubscriptionToken = aggregator.GetEvent<ACPacketEvent> ().Subscribe (HandleIncomingACMessage, ThreadOption.PublisherThread, true, f => f.Direction == Direction.ToController);
			setModeEventSubscriptionToken = aggregator.GetEvent<SetModeResponsePacketEvent> ().Subscribe (HandleIncomingSetModeResponse, ThreadOption.PublisherThread, true, f => f.Direction == Direction.FromController);
		} 

		private void OpenRfxDevice ()
		{
			this.rfxDevice.RfxDataReceived += RfxDataReceived;
			var physicalInterface = physicalInterfaceFactory.GetPhysicalInterface (configuration.ConnectionString);
			ProtocolReceiverSensitivityFlags sensitivityFlags = GetSensitivityFlags (packetTypes);
			rfxDevice.Open (physicalInterface, sensitivityFlags);
		}

	}

		



}
