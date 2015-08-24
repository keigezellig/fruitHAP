using Castle.Core.Logging;
using FruitHAP.Common.Configuration;
using FruitHAP.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.Controllers.ImageCaptureController.Configuration
{
    public class LocalImageCaptureControllerConfigurationProvider : ConfigProviderBase<LocalImageCaptureControllerConfiguration>
    {
        public LocalImageCaptureControllerConfigurationProvider(ILogger logger) : base(logger)
        {
        }

        protected override LocalImageCaptureControllerConfiguration LoadFromFile(string fileName)
        {
            return JsonSerializerHelper.Deserialize<LocalImageCaptureControllerConfiguration>(fileName);
        }

        protected override void SaveToFile(string fileName, LocalImageCaptureControllerConfiguration config)
        {
            JsonSerializerHelper.Serialize(fileName, config);
        }

        protected override LocalImageCaptureControllerConfiguration LoadDefaultConfig()
        {
            return new LocalImageCaptureControllerConfiguration() { TempPath = Path.GetTempFileName() };
        }

        public override bool IsConfigurationCorrect(LocalImageCaptureControllerConfiguration configuration)
        {
            return AreConfigurationItemsNotEmpty(configuration) && DoesCaptureCommandContainsCorrectItems(configuration.CaptureCommand);
        }

        private bool DoesCaptureCommandContainsCorrectItems(string captureCommand)
        {
            return (captureCommand.Contains("${resolution)") && captureCommand.Contains("${source)"));
        }

        private bool AreConfigurationItemsNotEmpty(LocalImageCaptureControllerConfiguration configuration)
        {
            return !string.IsNullOrEmpty(configuration.CaptureCommand) && !string.IsNullOrEmpty(configuration.TempPath);
        }
    }
}
