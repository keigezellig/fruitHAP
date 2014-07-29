using System;
using System.Configuration;
using System.IO;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Services.Logging.NLogIntegration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using EasyNetQ;
using PushBullet = EventNotifier.Plugins.PushBullet;
using EventNotifierService.Common.Plugin;
using EventNotifierService.Logging;
using EventNotifierService.Service;
using NLog;

namespace EventNotifierService.Startup
{
// ReSharper disable once UnusedMember.Global
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;            
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
                       
            RegisterLogging(container);
            RegisterMessageBus(container);

            string pluginDirectory = ConfigurationManager.AppSettings["PluginDirectory"] ?? Path.Combine(".","plugins");
            RegisterPlugins(container, pluginDirectory);

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(pluginDirectory))
                .BasedOn(typeof(IConfigProvider<>))
                .WithServiceBase());            
        }

        private static void RegisterPlugins(IWindsorContainer container, string pluginDirectory)
        {

            container.Register(
                          Component.For<PushBullet.IPushBulletService>().ImplementedBy<PushBullet.PushBulletService>().LifestyleTransient()
                          );

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(pluginDirectory))
                .BasedOn<IPlugin>()
                .WithService.FromInterface()
                .LifestyleTransient());

        }

        private void RegisterMessageBus(IWindsorContainer container)
        {
            var busBuilder = new BusBuilder(container);
            container.Register(
                Component.For<IEventNotifier>().ImplementedBy<EventNotifierService.Service.EventNotifier>().LifestyleTransient(),
                Component.For<IBus>().UsingFactoryMethod(busBuilder.CreateMessageBus).LifestyleSingleton()
                );
        }

        private void RegisterLogging(IWindsorContainer container)
        {
            container.AddFacility<LoggingFacility>(
                f => f.LogUsing(new NLogFactory(NLogConfigurationFactory.CreateNLogConfiguration())));
            container.Register(
                Component.For<LogFactory>()
                    .UsingFactoryMethod<LogFactory>(ServiceHostConfigurator.CreateLogFactoryForServiceHost),
                Component.For<IEasyNetQLogger>().ImplementedBy<EasyNetQLoggerAdapter>().LifestyleSingleton());
        }

        private void Kernel_ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            Console.WriteLine("Loading plugin: {0}",key);   
        }
    }

}
    

