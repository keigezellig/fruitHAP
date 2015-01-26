using System;
using Castle.Core.Logging;
using FruitHAP.SensorProcessing.Common;
using FruitHAP.SensorProcessing.Common.Configuration;
using FruitHAP.SensorProcessing.Common.InterfaceReaders;

namespace FruitHAP.SensorProcessing.SensorBinding.RfxBinding
{
    public class RfxBinding : IBinding
    {
        private readonly IProtocol protocol;
        private readonly IConfigProvider<RfxBindingConfiguration> configProvider;
        private readonly IInterfaceReaderFactory interfaceReaderFactory;
        private readonly ILogger logger;
        private RfxBindingConfiguration configuration;
        private IInterfaceReader interfaceReader;

        public RfxBinding(IRfxProtocolFactory protocol, IConfigProvider<RfxBindingConfiguration> configProvider, IInterfaceReaderFactory interfaceReaderFactory, ILogger logger)
        {
            this.protocol = protocol.CreateRfxProtocol();
            this.configProvider = configProvider;
            this.interfaceReaderFactory = interfaceReaderFactory;
            this.logger = logger;
        }

        void interfaceReader_DataReceived(object sender, ExternalDataReceivedEventArgs e)
        {
            protocol.Process(e.Data);
        }

        public void Start()
        {
            logger.InfoFormat("Starting binding {0}", this);

            try
            {
                this.configuration = configProvider.LoadConfigFromFile("RfxConfig.xml");
                interfaceReader = interfaceReaderFactory.CreateInterfaceReader(configuration.ConnectionString);
                interfaceReader.DataReceived += interfaceReader_DataReceived;
 
                interfaceReader.Open();
                interfaceReader.StartReading();
            }
            catch (Exception ex)
            {
                logger.Debug("Cannot start binding..",ex);
            }
           
        }

        public void Stop()
        {
           logger.InfoFormat("Stopping binding {0}",this);
            interfaceReader.StopReading();
           interfaceReader.Close();
        }

        public void Dispose()
        {
            logger.DebugFormat("Dispose binding {0}", this);
            interfaceReader.Dispose();
        }
    }
}
