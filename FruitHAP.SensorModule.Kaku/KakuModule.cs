using System;
using Castle.Core.Logging;
using FruitHAP.Common.Configuration;
using FruitHAP.Common.PhysicalInterfaces;
using FruitHAP.SensorModule.Kaku.Configuration;
using FruitHAP.SensorModule.Kaku.Protocol;

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

        void PhysicalInterfaceDataReceived(object sender, ExternalDataReceivedEventArgs e)
        {
            var data = protocol.Decode(e.Data);
            OnKakuDataReceived(data);
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
            logger.InfoFormat("Starting binding {0}", this);

            try
            {
                configuration = configProvider.LoadConfigFromFile("kaku.xml");
                physicalInterface = physicalInterfaceFactory.GetPhysicalInterface(configuration.ConnectionString);
                physicalInterface.DataReceived += PhysicalInterfaceDataReceived;

                physicalInterface.Open();
                physicalInterface.StartReading();
            }
            catch (Exception ex)
            {
                logger.Debug("Cannot start binding..", ex);
            }

        }

        public void Stop()
        {
            logger.InfoFormat("Stopping binding {0}", this);
            physicalInterface.StopReading();
            physicalInterface.Close();
        }

        public void Dispose()
        {
            logger.DebugFormat("Dispose binding {0}", this);
            physicalInterface.Dispose();
        }

        public event EventHandler<KakuProtocolEventArgs> KakuDataReceived;
    }

   
}
