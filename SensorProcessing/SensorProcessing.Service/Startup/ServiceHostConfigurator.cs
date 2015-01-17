using Castle.Windsor;
using NLog;
using SensorProcessing.Service.Service;
using Topshelf;
using Topshelf.HostConfigurators;

namespace SensorProcessing.Service.Startup
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
            configuration.Service<ISensorProcessingService>(s =>
            {
                s.ConstructUsing(name => container.Resolve<ISensorProcessingService>());
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
            configuration.SetDescription("Sensor processing service.");
            configuration.SetDisplayName("SensorProcessing.Service");
            configuration.SetServiceName("SensorProcessing.Service");
        }


        public static LogFactory CreateLogFactoryForServiceHost()
        {
            return new LogFactory(NLogConfigurationFactory.CreateNLogConfiguration());
        }
    }

    }

