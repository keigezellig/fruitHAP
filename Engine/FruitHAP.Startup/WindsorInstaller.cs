using System;
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
using FruitHAP.Core.Controller;
using FruitHAP.Core.SensorPersister;

using FruitHAP.Common.EventBus;

namespace FruitHAP.Startup
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {

			string controllerDirectory = ConfigurationManager.AppSettings["ControllerDirectory"] ??
                                     Path.Combine(".", "controllers");

			string sensorDirectory = ConfigurationManager.AppSettings["SensorDirectory"] ??
				Path.Combine(".", "sensors");

            string actionDirectory = ConfigurationManager.AppSettings["ActionDirectory"] ??
                                     Path.Combine(".", "actions");

            container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
            RegisterLogging(container);            
			RegisterMQPublisher(container);
			RegisterEventAggregator(container);            

			RegisterControllers(container,controllerDirectory);
			RegisterSensors(container,sensorDirectory);
            
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
               Component.For<ISensorRepository>()
                   .ImplementedBy<SensorRepository>()
                   .LifestyleSingleton());

			container.Register(
				Component.For<ISensorPersister>()
				.ImplementedBy<SensorPersister>()
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
            var logger = container.Resolve<Castle.Core.Logging.ILogger>();

            logger.InfoFormat("Loading actions from directory {0}", actionDirectory);
            
            container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(actionDirectory))
                .BasedOn<IAction>()
                .WithService.FromInterface()
                .LifestyleSingleton());

			container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(actionDirectory))
				.BasedOn(typeof (IConfigProvider<>))
				.WithService.Base()
				.LifestyleSingleton());
			

            logger.InfoFormat("Done loading actions from directory {0}", actionDirectory);

            
        }

        private void RegisterEventAggregator(IWindsorContainer container)
        {
            container.Register(
               Component.For<IEventAggregator>()
                   .ImplementedBy<EventAggregator>()
                   .LifestyleSingleton());

			container.Register(
				Component.For<IEventBus>()
				.ImplementedBy<PrismEventBus>()
				.LifestyleSingleton());

        }

        private void RegisterService(IWindsorContainer container)
        {
            container.Register(
                Component.For<ISensorProcessingService>()
                    .ImplementedBy<SensorProcessingService>()
                    .LifestyleTransient());

        }


		void RegisterControllers (IWindsorContainer container, string baseDirectory)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(baseDirectory);

			foreach (var directory in directoryInfo.GetDirectories())
			{
				LoadControllersFromDirectory(container, directory.FullName);    
			}
		}


		void RegisterSensors (IWindsorContainer container, string baseDirectory)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(baseDirectory);

			foreach (var directory in directoryInfo.GetDirectories())
			{
				LoadSensorsFromDirectory(container, directory.FullName);    
			}
		} 


		void LoadControllersFromDirectory (IWindsorContainer container, string controllerDirectory)
		{
			var logger = container.Resolve<Castle.Core.Logging.ILogger>();

			logger.InfoFormat("Loading controllers from directory {0}", controllerDirectory);

			container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(controllerDirectory))
				.BasedOn(typeof (IConfigProvider<>))
				.WithService.Base()
				.LifestyleSingleton());

			container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(controllerDirectory))
				.BasedOn<IPhysicalInterfaceFactory>()
				.WithService.FromInterface()
				.LifestyleSingleton());

			container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(controllerDirectory))
				.BasedOn<IController>()
				.WithService.AllInterfaces()
				.LifestyleSingleton());

			logger.InfoFormat("Done loading controllers from directory {0}", controllerDirectory);

		}

		void LoadSensorsFromDirectory (IWindsorContainer container, string sensorDirectory)
		{
			var logger = container.Resolve<Castle.Core.Logging.ILogger>();

			logger.InfoFormat("Loading sensors from directory {0}", sensorDirectory);

			container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(sensorDirectory))
				.BasedOn<ISensor>()
				.WithService.Base()
				.LifestyleSingleton());

			logger.InfoFormat("Done loading sensors from directory {0}", sensorDirectory);
		}

        private void RegisterLogging(IWindsorContainer container)
        {
			RegisterApplicationLogging (container);
			RegisterServiceHostLogging (container);
        }

		private void RegisterApplicationLogging (IWindsorContainer container)
		{
			container.AddFacility<LoggingFacility> (f => f.LogUsing (new NLogFactory (NLogConfigurationFactory.CreateNLogConfiguration ())));
		}

		private void RegisterServiceHostLogging (IWindsorContainer container)
		{
			container.Register (Component.For<LogFactory> ().UsingFactoryMethod<LogFactory> (ServiceHostConfigurator.CreateLogFactoryForServiceHost));
		}

        private void Kernel_ComponentRegistered(string key, IHandler handler)
        {            			
			Console.WriteLine("Loading component: {0}", key);
        }
    }
}
