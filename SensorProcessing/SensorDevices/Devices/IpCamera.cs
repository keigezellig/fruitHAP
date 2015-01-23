using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using SensorProcessing.Common.Device;

namespace SensorProcessing.SensorDevices.Devices
{
    public class IpCamera : ICamera
    {
        private readonly ILogger logger;
        private Uri url;
        private string username;
        private string password;

        public IpCamera(ILogger logger)
        {
            this.logger = logger;
        }

        public void Initialize(string name, string description, Uri url, string username = "", string password = "")
        {            
            Name = name;
            Description = description;
            this.url = url;
            this.username = username;
            this.password = password;

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

        public string Name { get; private set; }
        public string Description { get; private set; }

        public async Task<byte[]> GetImageAsync()
        {
            return await GetImageAsync(url, username, password);
        }
    }
}
