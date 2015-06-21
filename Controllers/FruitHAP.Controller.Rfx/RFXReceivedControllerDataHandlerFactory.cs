using System;
using System.Linq;
using FruitHAP.Core.Sensor.Controller;
using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;

namespace FruitHAP.Controller.Rfx
{
	public class RFXReceivedControllerDataHandlerFactory
	{
		private ILogger logger;
		IEventAggregator aggregator;
		private const byte Lighting2ProtocolIndicator = 0x11;
		private const byte ACPacketLength = 0x0B;
		private const byte ACPacketIndicator = 0x00;
		private const byte ClosingByte = 0x00;

		public RFXReceivedControllerDataHandlerFactory (ILogger logger, IEventAggregator aggregator)
		{
			this.aggregator = aggregator;
			this.logger = logger;
		}
		
		public IControllerPacketHandler CreateHandler(byte[] data)
		{
			if (IsThisAnACPacket (data)) 
			{
				return new ACPacketHandler (logger, aggregator);
			}

			return null;
		}


		private bool IsThisAnACPacket (byte[] data)
		{
			return IsTheLengthForAnACPacketCorrect (data) && IsThereAnACProtocolIndicator (data) && IsThereAnACPacketIndicator (data);
		}

		private bool IsThereAnACPacketIndicator(byte[] rawData)
		{
			if (rawData[2] != 0x00)
			{
				logger.DebugFormat("This is not an AC packet because the packet indiciator is incorrect. Packet indicator is 0x{0:X}",rawData[2]);
				return false;
			}

			return true;
		}

		private bool IsThereAnACProtocolIndicator(byte[] rawData)
		{
			if (rawData[1] != 0x11)
			{
				logger.DebugFormat("This is not an AC packet because the protocol byte is incorrect. Protocol indicator is 0x{0:X}", rawData[1]);
				return false;
			}

			return true;
		}

		private bool IsTheLengthForAnACPacketCorrect(byte[] rawData)
		{
			if (rawData [0] != ACPacketLength) 
			{
				logger.DebugFormat ("This is not an AC packet because of an incorrect length byte. Actual length byte={0}", rawData [0]);
				return false;
			}

			if (rawData.Count() != ACPacketLength + 1)
			{
				logger.DebugFormat ("This is not an AC packet because of an incorrect length. Actual packet length={0}", rawData.Count () - 1);
				return false;
			}

			return true;
		}
	}
}

