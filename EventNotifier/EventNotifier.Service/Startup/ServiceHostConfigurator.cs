using Castle.Windsor;
using EventNotifierService.Service;
using NLog;
using Topshelf;
using Topshelf.HostConfigurators;

namespace EventNotifierService.Startup
{
    public class ServiceHostConfigurator
    {
        private readonly IWindsorContainer container;

        public ServiceHostConfigurator(IWindsorContainer container)
        {
            this.container = container;
        }

        public void Configure(HostConfigurator configuration)
        {
            configuration.Service<IEventNotifier>(s =>
            {
                s.ConstructUsing(name => container.Resolve<IEventNotifier>());
                s.WhenStarted(tc => tc.Start());
                s.WhenStopped(tc =>
                {
                    tc.Stop();
                    container.Release(tc);
                    container.Dispose();
                });
            });

            configuration.UseLinuxIfAvailable();

            configuration.UseNLog(container.Resolve<LogFactory>());
            configuration.RunAsLocalSystem();
            configuration.SetDescription("Event notifier service.");
            configuration.SetDisplayName("EventNotifier.Service");
            configuration.SetServiceName("EventNotifier.Service");
        }


        public static LogFactory CreateLogFactoryForServiceHost()
        {
            return new LogFactory(NLogConfigurationFactory.CreateNLogConfiguration());
        }
    }
}

