namespace SensorBinding.Eventing
{
    public interface IPduPublisher<TPdu>
    {
        void Publish(TPdu pdu);
    }
}
