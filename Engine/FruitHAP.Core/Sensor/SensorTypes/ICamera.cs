using System.Threading.Tasks;
using FruitHAP.Core.Sensor.SensorValueTypes;

namespace FruitHAP.Core.Sensor.SensorTypes
{
    public interface ICamera : IValueSensor
    {
        Task<ImageValue> GetImageAsync();
    }
}
