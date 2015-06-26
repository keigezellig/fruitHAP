using System;
using System.Linq;
using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using System.Collections.Generic;
using FruitHAP.Controller.Rfx.Configuration;
using FruitHAP.Common.Configuration;
using FruitHAP.Core.Controller;
using FruitHAP.Controller.Rfx.PacketHandlers;
using FruitHAP.Controller.Rfx.InternalPacketData;

namespace FruitHAP.Controller.Rfx.PacketHandlers
{
	public class RfxInterfacePacketHandler : IControllerPacketHandler
	{
		private readonly ILogger logger;
		private IEventAggregator aggregator;
		private const byte STATUSCOMMAND = 0x02;
		private const byte SETMODECOMMAND = 0x03;
		private const byte COMMANDBYTE = 4;

		public RfxInterfacePacketHandler (ILogger logger, IEventAggregator aggregator)
		{
			this.logger = logger;
			this.aggregator = aggregator;
		}

		//SetMode response:
		//0D 01 00 04 03  53 E7 00 00 04 01 02 00 00
		//L  PT ST SQ CMD TT FV DV DV DV HW HW ? ?
		//(Same as GetStatus response, only CMD is different)

		#region IControllerPacketHandler implementation
		public void Handle (byte[] data)
		{
			StatusPacket decodedPacket = new StatusPacket ();
			decodedPacket.SequenceNumber = data [3];
			decodedPacket.DeviceType = (DeviceType)data [5];
			decodedPacket.FirmwareVersion = data [6];
			decodedPacket.HardwareVersion = string.Format ("{0}.{1}", data [10], data [11]);
			decodedPacket.ReceiverSensitivity = DecodeReceiverSensitivity (data);

			if (data [COMMANDBYTE] == STATUSCOMMAND) 
			{
				aggregator.GetEvent<StatusResponsePacketEvent> ().Publish (new ControllerEventData<StatusPacket> () {
					Direction = Direction.FromController,
					Payload = decodedPacket
				});
			}

			if (data [COMMANDBYTE] == SETMODECOMMAND) 
			{
				aggregator.GetEvent<SetModeResponsePacketEvent> ().Publish (new ControllerEventData<StatusPacket> () {
					Direction = Direction.FromController,
					Payload = decodedPacket
				});

			}
				
		}
		#endregion

		private ProtocolReceiverSensitivityFlags DecodeReceiverSensitivity (byte[] data)
		{
			var dataBytes = data.ToList();
			dataBytes.Insert(0, 0);
			var intValue = BitConverter.ToUInt32(dataBytes.AsEnumerable().Reverse().ToArray(), 0);
			ProtocolReceiverSensitivityFlags result = (ProtocolReceiverSensitivityFlags) intValue;
			return result;
		}
	}

}

