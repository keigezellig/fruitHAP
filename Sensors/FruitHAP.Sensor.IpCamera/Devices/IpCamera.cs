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

		private Uri uri;

        public IpCamera()
        {
            
        }
        public IpCamera(ILogger logger)
        {
            this.logger = logger;
        }

       
		public string Url {
			get 
			{
				return uri.ToString ();
			}
			set 
			{
				uri = new Uri (value);
			}
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
            return await GetImageAsync(uri, Username, Password);
        }


        public object Clone()
        {
            return new IpCamera(this.logger);
        }

		public object GetValue ()
		{
			return GetImageAsync ().Result;
		}


		public override string ToString ()
		{
			return string.Format ("[IpCamera: Name={0}, Description={1}, Url={2}, Username={3}, Password={4}]", Name, Description, Url, Username, Password);
		}
		
    }
}
