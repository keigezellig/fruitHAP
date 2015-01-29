﻿using System;
using System.Configuration;
using System.IO;
using Castle.Facilities.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Services.Logging.NLogIntegration;
using Castle.Windsor;
using FruitHAP.Core.Action;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.Service;
using Microsoft.Practices.Prism.PubSubEvents;
using NLog;

namespace FruitHAP.Startup
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
            RegisterEventAggregator(container);
            RegisterLogging(container);
            RegisterModules(container);
            RegisterDeviceRepository(container);
            RegisterActions(container);
            RegisterService(container);
            
        }

        private void RegisterDeviceRepository(IWindsorContainer container)
        {
            container.Register(
               Component.For<ISensorRepository>()
                   .ImplementedBy<SensorRepository>()
                   .LifestyleSingleton());
        }

        private void RegisterActions(IWindsorContainer container)
        {
            string actionDirectory = ConfigurationManager.AppSettings["ActionDirectory"] ??
                                     Path.Combine(".", "actions");

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(actionDirectory))
                .BasedOn<IAction>()
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

        private void RegisterModules(IWindsorContainer container)
        {
            string bindingDirectory = ConfigurationManager.AppSettings["ModuleDirectory"] ??
                                      Path.Combine(".", "modules");
        
            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(bindingDirectory))
                .BasedOn<ISensorModule>()
                .WithService.FromInterface()
                .LifestyleSingleton());

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(bindingDirectory))
               .BasedOn<ISensor>()
               .WithService.Base()
               .LifestyleTransient());
             
        }



        private void RegisterLogging(IWindsorContainer container)
        {
            container.AddFacility<LoggingFacility>(
                f => f.LogUsing(new NLogFactory(NLogConfigurationFactory.CreateNLogConfiguration())));
            container.Register(
                Component.For<LogFactory>()
                    .UsingFactoryMethod<LogFactory>(ServiceHostConfigurator.CreateLogFactoryForServiceHost));
        }

        private void Kernel_ComponentRegistered(string key, IHandler handler)
        {
            Console.WriteLine("Loading plugin: {0}", key);
        }
    }
}
