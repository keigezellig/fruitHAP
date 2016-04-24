using System;
using FruitHAP.Common.Configuration;
using Castle.Core.Logging;
using FruitHAP.Common.Helpers;

namespace FruitHap.Web.Startup
{
	public class WebConfigurationProvider : ConfigProviderBase<WebConfiguration>
	{
		public WebConfigurationProvider (ILogger logger) : base (logger)
		{
		}

		protected override WebConfiguration LoadFromFile(string fileName)
		{
			return JsonSerializerHelper.Deserialize<WebConfiguration>(fileName);
		}

		protected override void SaveToFile(string fileName, WebConfiguration config)
		{
			JsonSerializerHelper.Serialize(fileName,config);
		}

		protected override WebConfiguration LoadDefaultConfig()
		{
			return new WebConfiguration() { BaseUrl = "http://localhost:8888"};
		}

	}
}

