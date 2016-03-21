using System;
using Owin;
using System.Collections.Generic;
using System.Web.Http;
using FruitHAP.Core;
using OWIN.Windsor.DependencyResolverScopeMiddleware;

namespace FruitHap.RestInterface
{
	public class Startup
	{
		public void Configuration(IAppBuilder a)
		{
			a.UseWelcomePage(new Microsoft.Owin.Diagnostics.WelcomePageOptions() { Path = new Microsoft.Owin.PathString("/welcome")});
			ContainerAccessor.Container.Install (new ControllerInstaller ());
			HttpConfiguration httpConfiguration = new HttpConfiguration();
			Register(httpConfiguration);
			a.UseWindsorDependencyResolverScope (httpConfiguration, ContainerAccessor.Container);
			a.UseWebApi(httpConfiguration); 

			a.Run(context =>
				{
					context.Response.ContentType = "text/plain";

					string output = string.Format(
						"I'm running on {0} nFrom assembly {1}", 
						Environment.OSVersion, 
						System.Reflection.Assembly.GetEntryAssembly().FullName
					);

					return context.Response.WriteAsync(output);

				});
		}

		public static void Register(HttpConfiguration config)
		{			
			config.MapHttpAttributeRoutes ();
			/*config.Routes.MapHttpRoute(
				name: "Configuration",
				routeTemplate: "api/{controller}/{name}",
				defaults: new { controller = "Configuration", name = RouteParameter.Optional }
			);*/

			config.EnsureInitialized ();
		}
	}
}

