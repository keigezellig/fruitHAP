using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor;
using FruitHAP.Core.Sensor.SensorTypes;

namespace FruitHAP.SensorModule.IpCamera.Devices
{
    public class IpCamera : ICamera, ISensorInitializer, ICloneable
    {
        private readonly ILogger logger;
        private Uri url;
        private string username;
        private string password;

        public string Name { get; private set; }
        public string Description { get; private set; }

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
            username = parameters["Username"];
            password = parameters["Password"];
            url = new Uri(parameters["Url"]);
            
            logger.InfoFormat("Initialized camera {0}, {1}, {2}, {3}", Name, Description, username, password);
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
            return await GetImageAsync(url, username, password);
        }


        public object Clone()
        {
            return new IpCamera(this.logger);
        }
    }
}
