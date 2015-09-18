using System;
using FruitHAP.Core.Sensor.PacketData.ImageCapture;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace FruitHAP.Controllers.ImageCaptureController
{
    internal class HttpImageCapturer : IImageCapturer
    {
        public byte[] Capture(ImageRequestPacket request)
        {
            return GetImageAsync(new Uri(request.Uri), request.Username, request.Password).Result;
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


       
        public bool IsRequestOk(ImageRequestPacket request)
        {
            return true;
        }
    }
}