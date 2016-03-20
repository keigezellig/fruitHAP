using System;
using Nancy.Bootstrappers.Windsor;
using Castle.Windsor;

namespace FruitHap.RestInterface
{
	public class NancyBootstrapper : WindsorNancyBootstrapper
	{
		private static IWindsorContainer container;
		protected override IWindsorContainer GetApplicationContainer ()
		{
			return container;
		}

		public static void SetApplicationContainer(IWindsorContainer theContainer)
		{
			container = theContainer;
		}
	}
}

