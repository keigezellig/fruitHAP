using System.Threading.Tasks;

namespace FruitHAP.SensorProcessing.Common.Device
{
    public interface ICamera : IDevice
    {
        Task<byte[]> GetImageAsync();
    }
}
