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
using FruitHAP.Core.Sensor.PacketData.General;
using FruitHAP.Common.EventBus;

namespace FruitHAP.Controller.Rfx
{
	public class RfxController : BaseController
    {
        
		private const string CONFIG_FILENAME = "rfx.json";

		private readonly IConfigProvider<RfxControllerConfiguration> configProvider;
        private readonly IPhysicalInterfaceFactory physicalInterfaceFactory;
        private RfxControllerConfiguration configuration;
		private RfxControllerPacketHandlerFactory handlerFactory;
		private List<RfxPacketInfo> packetTypes;
		private RfxDevice rfxDevice;
        private RfxACProtocol protocol;

        Dictionary<int,ACPacket> acMessagesInProcess;


        public RfxController(ILogger logger, IEventBus eventBus, IConfigProvider<RfxControllerConfiguration> configProvider,
			IPhysicalInterfaceFactory physicalInterfaceFactory) : base(logger, eventBus)

        {            
            this.configProvider = configProvider;
            this.physicalInterfaceFactory = physicalInterfaceFactory;
			this.handlerFactory = new RfxControllerPacketHandlerFactory (logger, eventBus);
			this.rfxDevice = new RfxDevice (logger);
            this.acMessagesInProcess = new Dictionary<int, ACPacket>();
            this.protocol = new RfxACProtocol (logger);
        }


        protected override void StartController()
        {            
            LoadConfiguration();
            SubscribeToSensorEvents();
            OpenRfxDevice();
            SubscribeToRfxEvents();
        }

        protected override void StopController()
        {
            rfxDevice.Close();
        }

        protected override void DisposeController()
        {
			eventBus.Unsubscribe<ControllerEventData<ACPacket>>(HandleIncomingACMessage);
			eventBus.Unsubscribe<ControllerEventData<RfxStatusPacket>>(HandleIncomingSetModeResponse);
			eventBus.Unsubscribe<ControllerEventData<RfxAckPacket>>(HandleIncomingAckMessage);
            rfxDevice.Dispose();
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
				else
				{
					controllerPacketdHandler.Handle(e.Data);
				}

			} 
			catch (Exception ex) 
			{
				logger.ErrorFormat ("Error handling received data: {0}", ex.Message);
			}
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

			packetList.Add (GetInterfacePacket ());
			packetList.Add (GetAckPacket ());

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

		private RfxPacketInfo GetAckPacket()
		{
			return new RfxPacketInfo () {
				LengthByte = 0x04,
				PacketType = RfxPacketType.Ack,
				PacketIndicator = 0x02,
				SubPacketIndicator = 0x01
			};
		}

		private void HandleIncomingACMessage (ControllerEventData<ACPacket> obj)
		{			
            logger.FatalFormat("HandleIncomingACMessage called: {0}", obj);
            byte[] data = protocol.Encode (obj.Payload);
            try
            {
                int sequenceNumber = rfxDevice.SendData(data);
                acMessagesInProcess[sequenceNumber] = obj.Payload;
            }
            catch (Exception ex)
            {
                logger.Error("Error while sending data: ", ex);
                NakPacket<ControllerEventData<ACPacket>> nakPacket = new NakPacket<ControllerEventData<ACPacket>>() { Data = obj, Reason = NakReason.Failure };
                eventBus.Publish(nakPacket);
               
            }
		}

		private void HandleIncomingSetModeResponse (ControllerEventData<RfxStatusPacket> obj)
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

		void HandleIncomingAckMessage (ControllerEventData<RfxAckPacket> obj)
		{
			var responsePacket = obj.Payload;
            if (acMessagesInProcess.ContainsKey(responsePacket.SequenceNumber))
            {
                logger.Debug("**** publishing sensor event because of ack ***");
                eventBus.Publish(new ControllerEventData<ACPacket>() { Direction = Direction.FromController, Payload = acMessagesInProcess[responsePacket.SequenceNumber]});
                acMessagesInProcess.Remove(responsePacket.SequenceNumber);
            }			
		}

		private void LoadConfiguration ()
		{
			configuration = configProvider.LoadConfigFromFile (Path.Combine (Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location), CONFIG_FILENAME));
			packetTypes = LoadPacketTypes (configuration);
			handlerFactory.Initialize (packetTypes);
		}

		private void SubscribeToSensorEvents ()
		{
			eventBus.Subscribe<ControllerEventData<ACPacket>>(HandleIncomingACMessage, f => f.Direction == Direction.ToController);
		
		} 
        private void SubscribeToRfxEvents()
        {
            eventBus.Subscribe<ControllerEventData<RfxStatusPacket>>(HandleIncomingSetModeResponse, f => f.Direction == Direction.FromController && f.Payload.CommandType == CommandType.SetMode);
            eventBus.Subscribe<ControllerEventData<RfxAckPacket>>(HandleIncomingAckMessage, f => f.Direction == Direction.FromController);
        }

		private void OpenRfxDevice ()
		{
			this.rfxDevice.RfxDataReceived += RfxDataReceived;
			var physicalInterface = physicalInterfaceFactory.GetPhysicalInterface (configuration.ConnectionString);
			ProtocolReceiverSensitivityFlags sensitivityFlags = GetSensitivityFlags (packetTypes);
			rfxDevice.Open (physicalInterface, sensitivityFlags);
			System.Threading.Thread.Sleep (1000);
		}
        
    }

}
