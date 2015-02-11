using System;
using Castle.Core.Logging;
using FruitHAP.Common.Configuration;
using FruitHAP.Common.PhysicalInterfaces;
using FruitHAP.Core.Sensor;
using FruitHAP.SensorModule.Kaku.Configuration;
using FruitHAP.SensorModule.Kaku.Protocol;
using System.Reflection;
using System.IO;

namespace FruitHAP.SensorModule.Kaku
{
    public class KakuModule : IKakuModule
    {
        private readonly IConfigProvider<KakuConfiguration> configProvider;
        private readonly IPhysicalInterfaceFactory physicalInterfaceFactory;
        private readonly IKakuProtocol protocol;
        private readonly ILogger logger;
        private KakuConfiguration configuration;
        private IPhysicalInterface physicalInterface;
		private bool isStarted;

        public KakuModule(IConfigProvider<KakuConfiguration> configProvider, IPhysicalInterfaceFactory physicalInterfaceFactory, IKakuProtocol protocol, ILogger logger)
        {
            this.configProvider = configProvider;
            this.physicalInterfaceFactory = physicalInterfaceFactory;
            this.protocol = protocol;
            this.logger = logger;
        }

        public string Name
        {
            get { return "KlikAan KlikUit module"; }
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
            try
            {
                var data = protocol.Decode(e.Data);
				logger.DebugFormat("Decoded data: {0}",data);
				OnKakuDataReceived(data);
            }
            catch (ProtocolException ex)
            {                
                logger.ErrorFormat("Cannot decode packet. Error: {0}",ex.Message);
            }
        }

        protected virtual void OnKakuDataReceived(KakuProtocolData data)
        {            
            if (KakuDataReceived != null)
            {
                var localEvent = KakuDataReceived;
                localEvent(this,new KakuProtocolEventArgs() {Data = data});
            }

        }

        public void Start()
        {
            logger.InfoFormat("Starting module {0}", this);

            try
            {
				configuration = configProvider.LoadConfigFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"kaku.xml"));
                physicalInterface = physicalInterfaceFactory.GetPhysicalInterface(configuration.ConnectionString);
                physicalInterface.DataReceived += PhysicalInterfaceDataReceived;

                physicalInterface.Open();
                physicalInterface.StartReading();
				isStarted = true;
            }
            catch (Exception ex)
            {
                logger.Debug("Cannot start module..", ex);
				isStarted = false;
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

        public event EventHandler<KakuProtocolEventArgs> KakuDataReceived;
    }

   
}
