using Castle.Windsor;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Service;
using NLog;
using Topshelf;
using Topshelf.HostConfigurators;
using System;

namespace FruitHAP.Startup
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
			try
			{
            configuration.Service<IFruitHAPService>(s =>
            {
                s.ConstructUsing(name => container.Resolve<IFruitHAPService>());
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
			catch (Exception ex) 
			{
				Console.WriteLine (ex.Message);
			}
        }
		


        public static LogFactory CreateLogFactoryForServiceHost()
        {
            return new LogFactory(NLogConfigurationFactory.CreateNLogConfiguration());
        }
    }

    }

