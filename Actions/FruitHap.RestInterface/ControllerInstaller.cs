using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;

namespace FruitHap.RestInterface
{
	public class ControllerInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Classes.FromThisAssembly()
				.Pick().If(t => t.Name.EndsWith("Controller"))
				.Configure(configurer => configurer.Named(configurer.Implementation.Name))
				.LifestyleScoped());
		}
	}
}

