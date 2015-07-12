using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Core.Sensor.SensorTypes;
using System.Net.Http;

namespace FruitHAP.Sensor.IpCamera.Devices
{
	public class IpCamera : ICamera, ICloneable
    {
        private readonly ILogger logger;

        public string Name { get; set; }
        public string Description { get; set; }

        public IpCamera()
        {
            
        }
        public IpCamera(ILogger logger)
        {
            this.logger = logger;
        }

        public void Initialize(Dictionary<string, string> parameters)
        {
            Name = parameters["Name"];
            Description = parameters["Description"];
            Username = parameters["Username"];
            Password = parameters["Password"];
            Url = new Uri(parameters["Url"]);
            
            logger.InfoFormat("Initialized camera {0}, {1}, {2}, {3}", Name, Description, Username, Password);
        }

		public Uri Url {
			get;
			set;
		}

		public string Username {
			get;
			set;
		}

		public string Password {
			get;
			set;
		}

        private async Task<byte[]> GetImageAsync(Uri url, string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                if (!(string.IsNullOrEmpty(username)) || !(string.IsNullOrEmpty(password)))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            Encoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", username, password))));
                }

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync();
            }
        }

        

        public async Task<byte[]> GetImageAsync()
        {
            return await GetImageAsync(Url, Username, Password);
        }


        public object Clone()
        {
            return new IpCamera(this.logger);
        }

		public object GetValue ()
		{
			return GetImageAsync ().Result;
		}
    }
}
