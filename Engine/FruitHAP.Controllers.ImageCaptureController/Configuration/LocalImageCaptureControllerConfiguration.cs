using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruitHAP.Controllers.ImageCaptureController.Configuration
{
    public class LocalImageCaptureControllerConfiguration
    {
        public string TempPath { get; set; }
        public string CommandlineOptions { get; set; }
        public string PathToMPlayer { get; set; }
    }
}
