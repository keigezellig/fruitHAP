using Castle.Core.Logging;
using FruitHAP.Core.Sensor.PacketData.ImageCapture;
using System;
using System.Diagnostics;
using System.IO;

namespace FruitHAP.Controllers.ImageCaptureController
{

    internal class LocalImageCapturer : IImageCapturer
    {
        private ILogger logger;
        private const string URIPREFIX = "local://";

        public LocalImageCapturer(ILogger logger)
        {
            this.logger = logger.CreateChildLogger(this.GetType().Name);
        }
        

        public byte[] Capture(ImageRequestPacket request)
        {
            string stdOutput = "";
            string stdError = "";
            string tmpFile = string.Format("{0}.jpeg", Guid.NewGuid());

            string source = request.Uri.Remove(request.Uri.IndexOf(URIPREFIX), URIPREFIX.Length);
            string resolution = request.Resolution;

            string command = "/usr/bin/streamer";
            string commandArguments = string.Format("-c {0} -s {1} -o {2}", source, resolution, tmpFile);

            logger.DebugFormat("Command to be executed: {0} {1}", command, commandArguments);


            Process processToBeExecuted = new Process();

            processToBeExecuted.StartInfo.RedirectStandardOutput = true;
            processToBeExecuted.StartInfo.RedirectStandardError = true;
            processToBeExecuted.OutputDataReceived += (object sender, DataReceivedEventArgs e) => stdOutput += e.Data;
            processToBeExecuted.ErrorDataReceived += (object sender, DataReceivedEventArgs e) => stdError += e.Data;
            processToBeExecuted.StartInfo.FileName = command;
            processToBeExecuted.StartInfo.Arguments = commandArguments;
            processToBeExecuted.StartInfo.UseShellExecute = false;
            processToBeExecuted.Start();
            processToBeExecuted.BeginErrorReadLine();
            processToBeExecuted.BeginOutputReadLine();
            processToBeExecuted.WaitForExit();
            processToBeExecuted.Close();
            logger.Debug("Command STDOUT");
            logger.Debug(stdOutput);

            logger.Debug("Command STDERR");
            logger.Debug(stdError);

            if (!File.Exists(tmpFile))
            {
                logger.Error("Error while capturing image");
                return null;
            }

            var data = File.ReadAllBytes(tmpFile);

            File.Delete(tmpFile);

            return data;
        }

        public bool IsRequestOk(ImageRequestPacket request)
        {
            return !string.IsNullOrEmpty(request.Sender) && !string.IsNullOrEmpty(request.Resolution) && !string.IsNullOrEmpty(request.Uri.Substring(0, URIPREFIX.Length));
        }
    }
}