﻿using System;

namespace SensorProcessing.Common.InterfaceReaders.SerialPort
{
    public interface ISerialPort : IDisposable
    {
        void Open();
        void Close();
        bool IsOpen { get; }
        string ReadLine();
        void Write(byte[] buffer, int offset, int count);
        string NewLine { get; }
        string PortName { get; }
        int BytesToRead { get; }

        string ReadExisting();
    }
}