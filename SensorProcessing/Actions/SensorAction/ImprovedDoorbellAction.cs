using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using SensorProcessing.Common;
using SensorProcessing.Common.Device;

namespace SensorProcessing.SensorAction
{
    public class ImprovedDoorbellAction : ISensorAction
    {
        private readonly IDeviceRepository deviceRepository;
        private readonly ILogger logger;

        public ImprovedDoorbellAction(IDeviceRepository deviceRepository, ILogger logger)
        {
            this.deviceRepository = deviceRepository;
            this.logger = logger;
        }

        public void Initialize()
        {
            IButton doorbellButton = deviceRepository.FindDeviceOfTypeByName<IButton>("Doorbell");
            doorbellButton.ButtonPressed += doorbellButton_ButtonPressed;
        }

        private async void doorbellButton_ButtonPressed(object sender, EventArgs e)
        {
            logger.Info("Doorbell rang. Getting camera image");

            try
            {
                var image = await doorbellButton_ButtonPressedAsync();
                using (MemoryStream memoryStream = new MemoryStream(image))
                {
                    var bmpReturn = Image.FromStream(memoryStream);
                    bmpReturn.Save(@"C:\develop\image.jpg", ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error saving camera image",ex);
            }
        }

        private async Task<byte[]> doorbellButton_ButtonPressedAsync()
        {
            ICamera doorCamera = deviceRepository.FindDeviceOfTypeByName<ICamera>("DoorCamera");
            return await doorCamera.GetImageAsync();
        }
    }
}
