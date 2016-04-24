using System;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.MicroKernel.Registration;

namespace FruitHAP.Plugins.Web.Startup
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

