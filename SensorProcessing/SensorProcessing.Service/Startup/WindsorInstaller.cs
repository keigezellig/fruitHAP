using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Services.Logging.NLogIntegration;
using Castle.Windsor;
using Microsoft.Practices.Prism.PubSubEvents;
using NLog;
using SensorProcessing.Common;
using SensorProcessing.Common.Configuration;
using SensorProcessing.Common.InterfaceReaders;
using SensorProcessing.SensorBinding.RfxBinding;
using SensorProcessing.Service.Service;
using SensorProcessing.SensorAction;

namespace SensorProcessing.Service.Startup
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
            RegisterEventAggregator(container);
            RegisterLogging(container);
            RegisterBindings(container);
            RegisterActions(container);
            RegisterService(container);
            
        }

        private void RegisterActions(IWindsorContainer container)
        {
            string actionDirectory = ConfigurationManager.AppSettings["ActionDirectory"] ??
                                     Path.Combine(".", "actions");

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(actionDirectory))
                .BasedOn<ISensorAction>()
                .WithService.FromInterface()
                .LifestyleSingleton());
        }

        private void RegisterEventAggregator(IWindsorContainer container)
        {
            container.Register(
               Component.For<IEventAggregator>()
                   .ImplementedBy<EventAggregator>()
                   .LifestyleSingleton());
        }

        private void RegisterService(IWindsorContainer container)
        {
            container.Register(
                Component.For<ISensorProcessingService>()
                    .ImplementedBy<SensorProcessingService>()
                    .LifestyleTransient());

        }

        private void RegisterBindings(IWindsorContainer container)
        {
            string bindingDirectory = ConfigurationManager.AppSettings["BindingDirectory"] ??
                                      Path.Combine(".", "bindings");
        
            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(bindingDirectory))
                .BasedOn<IRfxProtocolFactory>()
                .WithService.FromInterface()
                .LifestyleSingleton());

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(bindingDirectory))
            .BasedOn(typeof(IConfigProvider<>))
            .WithServiceBase());

            container.Register(Classes.FromAssemblyContaining<IInterfaceReader>()
                .BasedOn<IInterfaceReaderFactory>()
                .WithService.FromInterface()
                .LifestyleSingleton());

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(bindingDirectory))
            .BasedOn<IBinding>()
            .WithService.FromInterface()
            .LifestyleSingleton());
        }



        private void RegisterLogging(IWindsorContainer container)
        {
            container.AddFacility<LoggingFacility>(
                f => f.LogUsing(new NLogFactory(NLogConfigurationFactory.CreateNLogConfiguration())));
            container.Register(
                Component.For<LogFactory>()
                    .UsingFactoryMethod<LogFactory>(ServiceHostConfigurator.CreateLogFactoryForServiceHost));
        }

        private void Kernel_ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            Console.WriteLine("Loading plugin: {0}", key);
        }
    }
}
