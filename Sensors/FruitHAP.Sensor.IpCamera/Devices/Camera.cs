using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using FruitHAP.Core.Sensor.SensorTypes;
using FruitHAP.Common.Helpers;
using FruitHAP.Core.Sensor.PacketData.ImageCapture;
using FruitHAP.Core.Controller;
using Microsoft.Practices.Prism.PubSubEvents;

namespace FruitHAP.Sensor.Camera.Devices
{
	public class Camera : ICamera, ICloneable
    {
        private readonly ILogger logger;
        private readonly IEventAggregator aggregator;
        private byte[] receivedImageData;
        private bool isReceived;

        private Uri uri;


        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Resolution { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Url
        {
            get
            {
                return uri.ToString();
            }
            set
            {
                uri = new Uri(value);
            }
        }


        public Camera(ILogger logger, IEventAggregator aggregator)
        {
            this.logger = logger;
            this.aggregator = aggregator;
            aggregator.GetEvent<ImageResponsePacketEvent>().Subscribe(HandleIncomingResponse, ThreadOption.PublisherThread, false, f => f.Direction == Direction.FromController && f.Payload.DestinationSensor == Name);
        }


        void HandleIncomingResponse(ControllerEventData<ImageResponsePacket> response)
        {
            this.isReceived = true;
            this.receivedImageData = response.Payload.ImageData;
        }



        public async Task<byte[]> GetImageAsync()
        {
            aggregator.GetEvent<ImageRequestPacketEvent>().Publish(new ControllerEventData<ImageRequestPacket>()
            {
                Direction = Direction.ToController,
                Payload = new ImageRequestPacket()
                {
                    Username = this.Username,
                    Password = this.Password,
                    Sender = this.Name,
                    Resolution = this.Resolution,                    
                    Uri = this.Url
                }
            });

            Task<byte[]> workerTask = new Task<byte[]>(() => {
                while (!isReceived)
                {
                }
                this.isReceived = false;
                return this.receivedImageData;
            });

            workerTask.Start();

            return await workerTask.TimeoutAfter(TimeSpan.FromSeconds(5));

        }


        public object Clone()
        {
            return new Camera(this.logger, this.aggregator);
        }

        public object GetValue()
        {
            return GetImageAsync().Result;
        }


        public override string ToString()
        {
            return string.Format("[IpCamera: Name={0}, Description={1}, Url={2}, UserName={3}, Password={4}]", Name, Description, Url, Username, Password);
        }
    }
}
