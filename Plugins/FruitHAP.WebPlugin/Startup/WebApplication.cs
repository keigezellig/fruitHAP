using System;
using Owin;
using FruitHAP.Core;
using System.Web.Http;
using OWIN.Windsor.DependencyResolverScopeMiddleware;
using WebApiContrib.Formatting.Jsonp;


namespace FruitHAP.Plugins.Web.Startup
{
	public class WebApplication
	{
		public void Configuration(IAppBuilder app)
		{
			#if DEBUG
				app.UseWelcomePage(new Microsoft.Owin.Diagnostics.WelcomePageOptions() { Path = new Microsoft.Owin.PathString("/welcome")});
			#endif

			ContainerAccessor.Container.Install (new ControllerInstaller ());
			var httpConfiguration = CreateHttpConfiguration ();
			app.UseWindsorDependencyResolverScope (httpConfiguration, ContainerAccessor.Container);
			app.UseWebApi(httpConfiguration); 
		}


		public HttpConfiguration CreateHttpConfiguration()
		{
			HttpConfiguration config = new HttpConfiguration();
			config.MapHttpAttributeRoutes ();
            config.AddJsonpFormatter();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
			config.EnsureInitialized ();
			return config;
		}

	}
}

