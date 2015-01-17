using System;
using Castle.Core.Logging;
using Castle.Windsor;
using Castle.Windsor.Installer;
using SensorProcessing.Common.InterfaceReaders;
using SensorProcessing.SensorBinding.RfxBinding;
using SensorProcessing.Service.Startup;
using Topshelf;

namespace SensorProcessing.Service
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            var container = new WindsorContainer().Install(FromAssembly.This());
            var serviceHostConfigurator = new ServiceHostConfigurator(container);
            HostFactory.Run(serviceHostConfigurator.Configure);

            //    ILogger logger = new ConsoleLogger();

            //    var binding = new RfxBinding(new RfxProtocolFactory(logger), new RfxBindingConfigurationProvider(logger),
            //        new InterfaceReaderFactory(logger), logger);

            //    ConsoleKeyInfo key = default(ConsoleKeyInfo);

            //    binding.Start();

            //    while (key.KeyChar != 'q')
            //    {
            //        key = Console.ReadKey();
            //        }

            //    binding.Stop();
            //    binding.Dispose();

            //}
        }
    }
}
