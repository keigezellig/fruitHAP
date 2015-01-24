using System.IO.Ports;

namespace SensorProcessing.Common.InterfaceReaders.SerialPort
{
    public sealed class SerialPort : ISerialPort
    {
        private readonly System.IO.Ports.SerialPort serialPort;

        public SerialPort(string portname, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            serialPort = new System.IO.Ports.SerialPort(portname, baudRate, parity, dataBits, stopBits);
            serialPort.NewLine = "\r";
        }
        public void Open()
        {
            serialPort.Open();
        }

        public void Close()
        {
            serialPort.Close();
        }

        public bool IsOpen
        {
            get { return serialPort.IsOpen; }
        }

        public string ReadLine()
        {
            return serialPort.ReadLine();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            serialPort.Write(buffer,offset,count);
        }

        public string NewLine
        {
            get
            {
                return serialPort.NewLine;
            }
        }

        public string PortName
        {
            get
            {
                return serialPort.PortName;
            }
        }

        public int BytesToRead
        {
            get
            {
                return serialPort.BytesToRead;
            }
        }

        public string ReadExisting()
        {
            return serialPort.ReadExisting();
        }

        public byte[] ReadBytesToEnd()
        {
            var bytesToRead = this.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            serialPort.Read(buffer, 0, bytesToRead);
            return buffer;
        }

        public void Dispose()
        {
            serialPort.Dispose();
        }
    }
}
