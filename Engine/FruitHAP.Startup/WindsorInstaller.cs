﻿using System;
using System.Configuration;
using System.IO;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Services.Logging.NLogIntegration;
using Castle.Windsor;
using FruitHAP.Common.Configuration;
using FruitHAP.Common.PhysicalInterfaces;
using FruitHAP.Core.Action;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.SensorRepository;
using FruitHAP.Core.Service;
using Microsoft.Practices.Prism.PubSubEvents;
using NLog;
using FruitHAP.Core;
using FruitHAP.Core.MQ;

namespace FruitHAP.Startup
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            string moduleDirectory = ConfigurationManager.AppSettings["ModuleDirectory"] ??
                                     Path.Combine(".", "modules");

            string actionDirectory = ConfigurationManager.AppSettings["ActionDirectory"] ??
                                     Path.Combine(".", "actions");

            container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
            RegisterLogging(container);            
			RegisterMQPublisher(container);
			RegisterEventAggregator(container);            
            RegisterModules(container,moduleDirectory);
            RegisterDeviceRepository(container);
            RegisterActions(container,actionDirectory);
            RegisterService(container);
            
        }

		void RegisterMQPublisher (IWindsorContainer container)
		{
			container.Register(
				Component.For<IMessageQueueProvider>()
				.ImplementedBy<RabbitMqProvider>()
				.LifestyleSingleton());
		}

        private void RegisterDeviceRepository(IWindsorContainer container)
        {
            container.Register(
                Component.For<ISensorLoader>()
                    .ImplementedBy<SensorLoader>()
                    .LifestyleSingleton());

            container.Register(
               Component.For<ISensorRepository>()
                   .ImplementedBy<SensorRepository>()
                   .LifestyleSingleton());

            
        }

        private void RegisterActions(IWindsorContainer container, string baseDirectory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(baseDirectory);

            foreach (var directory in directoryInfo.GetDirectories())
            {
                LoadActionFromDirectory(container, directory.FullName);
            }
        }

        private void LoadActionFromDirectory(IWindsorContainer container, string actionDirectory)
        {
            var logger = container.Resolve<ILogger>();

            logger.InfoFormat("Loading actions from directory {0}", actionDirectory);
            
            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(actionDirectory))
                .BasedOn<IAction>()
                .WithService.FromInterface()
                .LifestyleSingleton());

            logger.InfoFormat("Done loading actions from directory {0}", actionDirectory);

            
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

        private void RegisterModules(IWindsorContainer container, string baseDirectory)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(baseDirectory);

            foreach (var directory in directoryInfo.GetDirectories())
            {
                LoadModuleFromDirectory(container, directory.FullName);    
            }
            
        }

        private void LoadModuleFromDirectory(IWindsorContainer container, string moduleDirectory)
        {
            var logger = container.Resolve<ILogger>();

            logger.InfoFormat("Loading modules from directory {0}", moduleDirectory);
            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(moduleDirectory))
                .BasedOn(typeof (ISensorProtocol<>))
                .WithService.AllInterfaces()
                .LifestyleSingleton());

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(moduleDirectory))
                .BasedOn(typeof (IConfigProvider<>))
                .WithService.Base()
                .LifestyleSingleton());

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(moduleDirectory))
                .BasedOn<IPhysicalInterfaceFactory>()
                .WithService.FromInterface()
                .LifestyleSingleton());

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(moduleDirectory))
                .BasedOn<ISensorModule>()
                .WithService.AllInterfaces()
                .LifestyleSingleton());

            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(moduleDirectory))
                .BasedOn<ISensor>()
                .WithService.Base()
                .LifestyleSingleton());

            logger.InfoFormat("Done loading modules from directory {0}", moduleDirectory);
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
            Console.WriteLine("Loading component: {0}", key);
        }
    }
}