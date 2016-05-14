using FruitHAP.Core.Sensor.PacketData.ImageCapture;

namespace FruitHAP.Controllers.ImageCaptureController
{
    public interface IImageCapturer
    {
        byte[] Capture(ImageRequestPacket request);
        bool IsRequestOk(ImageRequestPacket request);
    }
}