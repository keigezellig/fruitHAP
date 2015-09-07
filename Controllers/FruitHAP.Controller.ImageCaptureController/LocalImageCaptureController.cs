using Castle.Core.Logging;
using FruitHAP.Common.Configuration;
using FruitHAP.Controllers.ImageCaptureController.Configuration;
using FruitHAP.Core.Controller;
using FruitHAP.Core.Sensor.PacketData.ImageCapture;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using FruitHAP.Common;

namespace FruitHAP.Controllers.ImageCaptureController
{
    public class LocalImageCaptureController : BaseController
    {
        private SubscriptionToken subscriptionToken;

		private const string URIPREFIX = "local://";

        public LocalImageCaptureController(ILogger logger, IEventAggregator aggregator) : base(logger, aggregator)
        {
        }
        public override string Name
        {
            get
            {
                return "Local image capture controller";
            }
        }

        protected override void DisposeController()
        {
            UnSubscribe();
        }

        protected override void StartController()
        {
            Subscribe();
        }

        private void Subscribe()
        {            
            logger.Debug("Subscribing to sensor request");
            subscriptionToken = aggregator.GetEvent<ImageRequestPacketEvent>().Subscribe(HandleImageRequestPacket, ThreadOption.PublisherThread, true, f => f.Direction == Direction.ToController && f.Payload.Uri.StartsWith("local://"));            
        }

        private void UnSubscribe()
        {
            if (subscriptionToken != null)
            {
                logger.Debug("Unsubscribing from sensor request");
                aggregator.GetEvent<ImageRequestPacketEvent>().Unsubscribe(subscriptionToken);
            }

        }

		private byte[] CaptureImage(string source, string resolution)
		{
			string output = "";
			string error = "";
			string tmpFile = string.Format ("{0}.jpeg", Guid.NewGuid ());

			string command = "/usr/bin/streamer";
			string commandArguments = string.Format ("-c {0} -s {1} -o {2}", source, resolution, tmpFile);

			logger.DebugFormat("Command to be executed: {0} {1}",command,commandArguments);


			Process processToBeExecuted = new Process ();
			processToBeExecuted.EnableRaisingEvents = true;
			processToBeExecuted.StartInfo.RedirectStandardOutput = false;
			processToBeExecuted.StartInfo.RedirectStandardError = false;
			processToBeExecuted.OutputDataReceived += (object sender, DataReceivedEventArgs e) => output += e.Data;
			processToBeExecuted.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => error += e.Data;
			processToBeExecuted.StartInfo.FileName = command;
			processToBeExecuted.StartInfo.Arguments = commandArguments;
			processToBeExecuted.StartInfo.UseShellExecute = false;
			processToBeExecuted.Start ();
			processToBeExecuted.WaitForExit();
			processToBeExecuted.Close ();
			logger.Debug ("Command STDOUT");
			logger.Debug (output);

			logger.Debug ("Command STDERR");
			logger.Debug (error);

			if (!File.Exists (tmpFile))
			{
				logger.Error ("Error while capturing image");
				return null;
			}

			var data = File.ReadAllBytes (tmpFile);

			File.Delete (tmpFile);

			return data;
		}

        private void HandleImageRequestPacket(ControllerEventData<ImageRequestPacket> request)
        {            

			try
			{
			if (!IsRequestOk (request.Payload)) 
			{
				//TODO: Introduce validator class
				logger.Error ("Invalid request");
				return;
			}

			string source = request.Payload.Uri.Remove (request.Payload.Uri.IndexOf (URIPREFIX), URIPREFIX.Length);
			var image = CaptureImage (source, request.Payload.Resolution);
				

			aggregator.GetEvent<ImageResponsePacketEvent> ().Publish (new ControllerEventData<ImageResponsePacket> () {
				Direction = Direction.FromController,
				Payload = new ImageResponsePacket () {  DestinationSensor = request.Payload.Sender, ImageData = image }  
 			});
			}
			catch (Exception ex) 
			{
				logger.Error ("Error capturing image: ", ex);
				throw;
			}
		}


		bool IsRequestOk (ImageRequestPacket request)
		{
			return !string.IsNullOrEmpty (request.Sender) && !string.IsNullOrEmpty (request.Resolution) && !string.IsNullOrEmpty (request.Uri.Substring (0, URIPREFIX.Length));
		}


        protected override void StopController()
        {
			UnSubscribe ();
        }
    }
}
