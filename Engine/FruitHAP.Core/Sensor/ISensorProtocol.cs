namespace FruitHAP.Core.Sensor
{
    public interface ISensorProtocol<T>
    {
        T Decode(byte[] rawData);
        byte[] Encode(T data);
    }
}