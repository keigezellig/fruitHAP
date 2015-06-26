using System;
using FruitHAP.Common.PhysicalInterfaces;
using Castle.Core.Logging;
using System.Collections.Generic;
using System.Linq;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Controller.Rfx
{
	public class RfxDevice : IDisposable
	{
		private IPhysicalInterface physicalInterface;
		private ILogger logger;
		private byte sequenceNumber;
		private byte previousSequenceNumber;

		public event EventHandler<ExternalDataReceivedEventArgs> RfxDataReceived;

		public RfxDevice (ILogger logger)
		{
			this.logger = logger;

		}

		public void Open(IPhysicalInterface physicalInterface, ProtocolReceiverSensitivityFlags sensitivityFlags)
		{
			logger.InfoFormat ("Opening Rfx device with sensitivity flags {0}",sensitivityFlags); 
			this.physicalInterface = physicalInterface;

			physicalInterface.DataReceived += PhysicalInterfaceDataReceived;
			physicalInterface.Open ();
			physicalInterface.StartReading ();

			SendResetCommand();
			SendModeCommand(sensitivityFlags);
		}



		public void SendData (byte[] data)
		{			
			List<byte> bytesToBeSend = new List<byte> (data);
			bytesToBeSend.Insert (0, (byte)data.Count ());
			bytesToBeSend[3] = sequenceNumber;
			logger.DebugFormat ("Sending bytes {0} to controller", data.BytesAsString ());
			physicalInterface.Write(bytesToBeSend.ToArray());
			previousSequenceNumber = sequenceNumber;
			sequenceNumber++;
		}

		//0D 00 00 00 00 00 00 00 00 00 00 00 00 00 
		public void SendResetCommand()
		{
			logger.Debug ("Sending reset command to device");
			var dataToBeSend = new byte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
			physicalInterface.Write(dataToBeSend);
		}

		//0D 00 00 SEQ 03 53 00 SB1 SB2 SB3 00 00 00 00
		public void SendModeCommand(ProtocolReceiverSensitivityFlags protocolReceiverSensitivity)
		{
			logger.Debug ("Sending mode command to device");
			byte[] sensitivityBytes = GetSensitivityBytes (protocolReceiverSensitivity);
			List<byte> dataToBeSend = new List<byte> ();
			dataToBeSend.AddRange(new byte[] {0x00,0x00,0xFF,0x03,0x53,0x00});
			dataToBeSend.AddRange (sensitivityBytes);
			dataToBeSend.AddRange (new byte[] { 0x00, 0x00, 0x00, 0x00 });
			SendData (dataToBeSend.ToArray());
		}

		public byte  PreviousSequenceNumber 
		{
			get
			{
				return previousSequenceNumber;
			}
		}



		public void Close()
		{
			physicalInterface.StopReading();
			physicalInterface.Close();
		}

		public void Dispose()
		{
			physicalInterface.Dispose();
		}


		private byte[] GetSensitivityBytes (ProtocolReceiverSensitivityFlags protocolReceiverSensitivityFlags)
		{
			uint intvalue = (uint) protocolReceiverSensitivityFlags;
			return BitConverter.GetBytes(intvalue).Reverse().Skip(1).ToArray();
		}

		private void PhysicalInterfaceDataReceived (object sender, ExternalDataReceivedEventArgs e)
		{
			if (RfxDataReceived != null) 
			{
				var localEvent = RfxDataReceived;
				localEvent (this, e);
			}
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


