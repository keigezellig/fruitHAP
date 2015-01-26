using System;

namespace FruitHAP.SensorProcessing.Common.InterfaceReaders
{
    public interface IInterfaceReader : IDisposable
    {
        void Open();
        void Close();
        void StartReading();
        void StopReading();
        event EventHandler<ExternalDataReceivedEventArgs> DataReceived;
        void Write(byte[] data);
    }

    public class ExternalDataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; set; }
    }
}
