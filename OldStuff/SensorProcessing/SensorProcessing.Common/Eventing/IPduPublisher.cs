namespace FruitHAP.SensorProcessing.Common.Eventing
{
    public interface IPduPublisher<TPdu>
    {
        void Publish(TPdu pdu);
    }
}
