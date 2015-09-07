using System;
using FruitHAP.Core.Sensor.SensorTypes;
using Castle.Core.Logging;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.Sensor.PacketData.ImageCapture;
using FruitHAP.Core.Controller;
using FruitHAP.Common.Helpers;

namespace FruitHAP.Sensor.IpCamera
{
	public class UsbCamera : ICamera
	{		
		private readonly ILogger logger;
		private readonly IEventAggregator aggregator;

		public string Name { get; set; }
		public string Description { get; set; }
		public string Resolution { get; set; }

		private byte[] receivedImageData;
		private bool isReceived;

		private Uri uri;

		public UsbCamera (ILogger logger, IEventAggregator aggregator)
		{
			this.logger = logger;
			this.aggregator = aggregator;
			aggregator.GetEvent<ImageResponsePacketEvent> ().Subscribe (HandleIncomingResponse, ThreadOption.PublisherThread, false, f => f.Direction == Direction.FromController && f.Payload.DestinationSensor == Name); 

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


		void HandleIncomingResponse (ControllerEventData<ImageResponsePacket> response)
		{
			this.isReceived = true;
			this.receivedImageData = response.Payload.ImageData;
		}



		public async Task<byte[]> GetImageAsync()
		{
			aggregator.GetEvent<ImageRequestPacketEvent> ().Publish (new ControllerEventData<ImageRequestPacket> () {
				Direction = Direction.ToController,
				Payload = new ImageRequestPacket () {
					Sender = this.Name,
					Resolution = this.Resolution,
					Uri = this.Url
				}
			});	

			Task<byte[]> workerTask = new Task<byte[]> (() => {
				while (!isReceived)
				{
				}
				this.isReceived = false;
				return this.receivedImageData;
			});

			workerTask.Start ();

			return await workerTask.TimeoutAfter(TimeSpan.FromSeconds(5));

		}


		public object Clone()
		{
			return new UsbCamera(this.logger, this.aggregator);
		}

		public object GetValue ()
		{
			return GetImageAsync ().Result;
		}


		public override string ToString ()
		{
			return string.Format ("[UsbCamera: Name={0}, Description={1}, Url={2}]", Name, Description, Url);
		}	}
}

