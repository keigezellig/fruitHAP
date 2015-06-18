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

namespace FruitHAP.Controller.Rfx
{
	public class RfxController : IACController
    {
        private readonly IConfigProvider<RfxControllerConfiguration> configProvider;
        private readonly IPhysicalInterfaceFactory physicalInterfaceFactory;
        private readonly ILogger logger;
        private RfxControllerConfiguration configuration;
        private IPhysicalInterface physicalInterface;
		private bool isStarted;
		private static byte SequenceNumber = 1;
		private ISensorProtocol<ACProtocolData> protocol;

		private const string CONFIG_FILENAME = "rfx.xml";

		public RfxController(IConfigProvider<RfxControllerConfiguration> configProvider, IPhysicalInterfaceFactory physicalInterfaceFactory, ILogger logger, ISensorProtocol<ACProtocolData> protocol)
        {
            this.configProvider = configProvider;
            this.physicalInterfaceFactory = physicalInterfaceFactory;
            this.logger = logger;
			this.protocol = protocol;
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
				var decodedData = protocol.Decode (e.Data);
				if (ACDataReceived != null)
				{
					var @event = ACDataReceived;
					@event(this,new ACProtocolEventArgs() {Data = decodedData});
				}
			}
			catch (ProtocolException ex) 
			{
				logger.ErrorFormat ("Error decoding received data: {0}", ex.Message);
			}
        }

		public event EventHandler<ACProtocolEventArgs> ACDataReceived;
		
		public void SendACData (ACProtocolData data)
		{
			try
			{
				var dataBytes = protocol.Encode(data);
				SendData(dataBytes);
			}
			catch (ProtocolException ex) 
			{
				logger.ErrorFormat ("Error encoding data: {0}", ex.Message);
			}
		
		}
       

        public void Start()
        {
			if (!IsStarted) {
				logger.InfoFormat ("Initializing controller {0}", this);

				try {
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
