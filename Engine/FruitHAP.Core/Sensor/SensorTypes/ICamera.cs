using System.Threading.Tasks;

namespace FruitHAP.Core.Sensor.SensorTypes
{
    public interface ICamera : ISensor
    {
        Task<byte[]> GetImageAsync();
    }
}
