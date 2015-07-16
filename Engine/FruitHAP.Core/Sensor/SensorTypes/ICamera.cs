using System.Threading.Tasks;

namespace FruitHAP.Core.Sensor.SensorTypes
{
    public interface ICamera : IValueSensor
    {
        Task<byte[]> GetImageAsync();
    }
}
