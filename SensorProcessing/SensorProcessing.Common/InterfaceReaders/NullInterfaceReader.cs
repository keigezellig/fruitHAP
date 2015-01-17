using System;
using Castle.Core.Logging;

namespace SensorProcessing.Common.InterfaceReaders
{
    public class NullInterfaceReader : IInterfaceReader
    {
        private readonly ILogger log;

        public NullInterfaceReader(ILogger log)
        {
            this.log = log;
        }

        public void Open()
        {
            log.Fatal("Invalid connection string in configuration");
        }

        public void Close()
        {
            log.Fatal("Invalid connection string in configuration");
        }

        public void StartReading()
        {
            log.Fatal("Invalid connection string in configuration");
        }

        public void StopReading()
        {
            log.Fatal("Invalid connection string in configuration");
        }

        public event EventHandler<ExternalDataReceivedEventArgs> DataReceived;

        public void Write(byte[] data)
        {
            log.Fatal("Invalid connection string in configuration");
        }

        public void Dispose()
        {
            log.Fatal("Invalid connection string in configuration");
        }
    }
}
