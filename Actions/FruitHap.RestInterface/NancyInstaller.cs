﻿using System;
using Castle.MicroKernel.Registration;

namespace FruitHap.RestInterface
{
	public class NancyInstaller : IWindsorInstaller
	{
		public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
		{
			NancyBootstrapper.SetApplicationContainer(container);
		}
	}
}

