using System;
using System.Linq;
using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Collections.Generic;
using FruitHAP.Controller.Rfx.Configuration;
using FruitHAP.Common.Configuration;
using FruitHAP.Core.Controller;
using FruitHAP.Controller.Rfx.PacketHandlers;
using FruitHAP.Common.EventBus;

namespace FruitHAP.Controller.Rfx
{
	public class RfxControllerPacketHandlerFactory
	{
		private ILogger logger;
		private IEventBus eventBus;
		private List<RfxPacketInfo> enabledPacketTypes;



		public RfxControllerPacketHandlerFactory (ILogger logger, IEventBus eventBus)
		{
			this.eventBus = eventBus;
			this.logger = logger;
		}
		
		public IControllerPacketHandler CreateHandler(byte[] data)
		{
			var packetType = GetPacketType (data);

			switch (packetType) 
			{
			case RfxPacketType.AC:
				logger.Debug ("AC packet received");
				return new RfxACPacketHandler (logger, eventBus);
			case RfxPacketType.RfxSensorTemperature:
				logger.Debug ("RfxSensorTemperature packet received");
				return new RfxRfxSensorTemperaturePacketHandler (logger, eventBus);
			case RfxPacketType.RfxSensorMessage:
				logger.Debug ("RfxSensorMessagePacket received");
				return new RfxRfxSensorMessagePacketHandler (logger, eventBus);
			case RfxPacketType.RfxSensorVoltage:
				logger.Debug ("RfxSensorVoltage packet received");
				return new RfxRfxSensorVoltagePacketHandler (logger, eventBus);
            case RfxPacketType.RfxMeter:
                logger.Debug ("RfxMeter packet received");
                return new RfxRfxMeterPacketHandler (logger, eventBus);
			case RfxPacketType.Interface:
				logger.Debug ("Interface packet received");
				return new RfxInterfacePacketHandler (logger, eventBus);
			case RfxPacketType.Ack:
				logger.Debug ("Ack packet received");
				return new RfxAckPacketHandler (logger, eventBus);

			default:
				return null;
			}
		}

		public void Initialize(List<RfxPacketInfo> enabledPacketTypes)
		{
			this.enabledPacketTypes = enabledPacketTypes;
		}

		private RfxPacketType GetPacketType(byte[] data)
		{

			foreach (var type in enabledPacketTypes) 
			{
				if (IsCorrectLength (data, type) && IsCorrectPacketIndicator (data, type) && IsCorrectSubPacketIndicator (data, type)) 
				{
					return type.PacketType;
				}
			}

			return RfxPacketType.Unknown;
		}

		private bool IsCorrectLength (byte[] data, RfxPacketInfo type)
		{
			return (data [0] == type.LengthByte) && (data.Count() == type.LengthByte + 1);
		}	
		private bool IsCorrectPacketIndicator (byte[] data, RfxPacketInfo type)
		{
			return data [1] == type.PacketIndicator;
		}

		private bool IsCorrectSubPacketIndicator (byte[] data, RfxPacketInfo type)
		{
			return data [2] == type.SubPacketIndicator;
		}
	}
}

