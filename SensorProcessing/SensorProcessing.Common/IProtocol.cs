namespace SensorProcessing.Common
{
    public interface IProtocol
    {
        void Process(byte[] input);
    }
}