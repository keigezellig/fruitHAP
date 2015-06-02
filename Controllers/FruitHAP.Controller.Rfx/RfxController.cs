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

namespace FruitHAP.Controller.Rfx
{
	public class RfxController : IRfxController
    {
        private readonly IConfigProvider<RfxControllerConfiguration> configProvider;
        private readonly IPhysicalInterfaceFactory physicalInterfaceFactory;
        private readonly ILogger logger;
        private RfxControllerConfiguration configuration;
        private IPhysicalInterface physicalInterface;
		private bool isStarted;
		private static byte SequenceNumber = 1;

		private const string CONFIG_FILENAME = "rfx.xml";

        public RfxController(IConfigProvider<RfxControllerConfiguration> configProvider, IPhysicalInterfaceFactory physicalInterfaceFactory, ILogger logger)
        {
            this.configProvider = configProvider;
            this.physicalInterfaceFactory = physicalInterfaceFactory;
            this.logger = logger;
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
            
			if (ControllerDataReceived != null)
			{
				var localEvent = ControllerDataReceived;
				localEvent(this,new ControllerDataEventArgs() {Data = e.Data});
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

		public event EventHandler<ControllerDataEventArgs> ControllerDataReceived;

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
			logger.Warn ("Sending reset command to controller (NOT IMPLEMENTED YET)");
		}
    }

   
}
