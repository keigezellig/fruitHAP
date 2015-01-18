﻿namespace SensorProcessing.Common.InterfaceReaders
{
    public interface IInterfaceReaderFactory
    {
        IInterfaceReader CreateInterfaceReader(string connectionString);
    }
}