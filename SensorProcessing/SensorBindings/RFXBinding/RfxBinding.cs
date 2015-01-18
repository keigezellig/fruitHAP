using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging; 
using SensorProcessing.Common;
using SensorProcessing.Common.Configuration;
using SensorProcessing.Common.InterfaceReaders;

namespace SensorProcessing.SensorBinding.RfxBinding
{
    public class RfxBinding : IBinding
    {
        private readonly IProtocol protocol;
        private readonly ILogger logger;
        private RfxBindingConfiguration configuration;
        private readonly IInterfaceReader interfaceReader;

        public RfxBinding(IRfxProtocolFactory protocol, IConfigProvider<RfxBindingConfiguration> configProvider, IInterfaceReaderFactory interfaceReaderFactory, ILogger logger)
        {
            this.protocol = protocol.CreateRfxProtocol();
            this.logger = logger;
            this.configuration = configProvider.LoadConfigFromFile("RfxConfig.xml");
            interfaceReader = interfaceReaderFactory.CreateInterfaceReader(configuration.ConnectionString);
            interfaceReader.DataReceived += interfaceReader_DataReceived;
        }

        void interfaceReader_DataReceived(object sender, ExternalDataReceivedEventArgs e)
        {
            protocol.Process(e.Data);
        }

        public void Start()
        {
            try
            {
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
           interfaceReader.StopReading();
           interfaceReader.Close();
        }

        public void Dispose()
        {
            interfaceReader.Dispose();
        }
    }
}
