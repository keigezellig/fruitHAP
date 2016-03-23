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
using FruitHAP.Common.EventBus;
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHAP.Sensor.Camera.Devices
{
	public class Camera : ICamera
    {
        private readonly ILogger logger;
		private readonly IEventBus eventBus;
        private ImageValue receivedImageData;
        private bool isReceived;

        private Uri uri;
		private DateTime lastUpdateTime;

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


        public Camera(ILogger logger, IEventBus eventBus)
        {
            this.logger = logger;
			this.eventBus = eventBus;
			this.lastUpdateTime = DateTime.Now;
			eventBus.Subscribe<ControllerEventData<ImageResponsePacket>>(HandleIncomingResponse, f => f.Direction == Direction.FromController && f.Payload.DestinationSensor == Name);
        }


        void HandleIncomingResponse(ControllerEventData<ImageResponsePacket> response)
        {
            this.isReceived = true;
			this.receivedImageData = new ImageValue () { ImageData = response.Payload.ImageData };
			this.lastUpdateTime = DateTime.Now;
        }



        public async Task<ImageValue> GetImageAsync()
        {
			eventBus.Publish(new ControllerEventData<ImageRequestPacket> ()
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

			Task<ImageValue> workerTask = new Task<ImageValue>(() => {
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
            return new Camera(this.logger, this.eventBus);
        }

		public ISensorValueType GetValue()
        {
            return GetImageAsync().Result;
        }

		public DateTime GetLastUpdateTime ()
		{
			return lastUpdateTime;
		}


        public override string ToString()
        {
            return string.Format("[IpCamera: Name={0}, Description={1}, Url={2}, UserName={3}, Password={4}]", Name, Description, Url, Username, Password);
        }

		public void Dispose ()
		{
			eventBus.Unsubscribe<ControllerEventData<ImageResponsePacket>> (HandleIncomingResponse);
		}
    }
}
