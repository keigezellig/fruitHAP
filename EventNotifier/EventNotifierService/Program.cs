using EventNotifierService.Startup;
using Topshelf;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace EventNotifierService
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var container = new WindsorContainer().Install(FromAssembly.This());
            var serviceHostConfigurator = new ServiceHostConfigurator(container);
            HostFactory.Run(serviceHostConfigurator.Configure);
        }
    }
}
