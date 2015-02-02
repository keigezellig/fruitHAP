using Castle.Windsor;
using Castle.Windsor.Installer;
using FruitHAP.SensorProcessing.Service.Startup;
using Topshelf;

namespace FruitHAP.SensorProcessing.Service
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new WindsorContainer().Install(FromAssembly.This());
            var serviceHostConfigurator = new ServiceHostConfigurator(container);
            HostFactory.Run(serviceHostConfigurator.Configure);
        }
    }
}
