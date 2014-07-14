using System;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Services.Logging.NLogIntegration;
using Castle.Windsor;
using EasyNetQ;
using EventNotifierService.Logging;
using EventNotifierService.Messages;
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
           
            var busBuilder = new BusBuilder(container);
            container.AddFacility<LoggingFacility>(
                f => f.LogUsing(new NLogFactory(NLogConfigurationFactory.CreateNLogConfiguration())));
            container.Register(
                Component.For<IEventNotifier>().ImplementedBy<EventNotifier>().LifestyleTransient(),
                Component.For<IBus>().UsingFactoryMethod(busBuilder.CreateMessageBus).LifestyleSingleton(),
                Component.For<LogFactory>()
                    .UsingFactoryMethod<LogFactory>(ServiceHostConfigurator.CreateLogFactoryForServiceHost),
                Component.For<IEasyNetQLogger>().ImplementedBy<EasyNetQLoggerAdapter>().LifestyleSingleton()
                );

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(@"C:\develop\projects\EventNotifier\EventNotifier\Plugins\bin"))
                .IncludeNonPublicTypes()
                .BasedOn<IMessageHandler>()
                .WithService.FromInterface()
                .LifestyleTransient());

           



            
        }

        public void Kernel_ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            Console.WriteLine("Registering component {0}",key);   
        }
    }

}
    

