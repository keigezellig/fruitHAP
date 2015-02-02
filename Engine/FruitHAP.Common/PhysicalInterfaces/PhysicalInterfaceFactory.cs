using System;
using System.IO.Ports;
using System.Linq;
using Castle.Core.Logging;
using FruitHAP.Common.PhysicalInterfaces.SerialPortInterface;

namespace FruitHAP.Common.PhysicalInterfaces
{
    public class PhysicalInterfaceFactory : IPhysicalInterfaceFactory
    {
        private readonly ILogger logger;

        public PhysicalInterfaceFactory(ILogger logger)
        {
            this.logger = logger;
        }

        public IPhysicalInterface GetPhysicalInterface(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return new NullPhysicalInterface(logger);

            var items = connectionString.Split(new string[] {":"},StringSplitOptions.RemoveEmptyEntries);
            var typeIdentifier = items[0];
            var parameterString = items[1];

            return CreateReader(typeIdentifier, parameterString);
         
        }

        private IPhysicalInterface CreateReader(string typeIdentifier, string parameterString)
        {
            if (typeIdentifier == "serial")
            {
                //COM1,38400,8,N,1
                var parameters = parameterString.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
                if (parameters.Count() != 5)
                {
                    return new NullPhysicalInterface(logger);                    
                }

                string portname = parameters[0];

                try
                {
                    int baudrate = ParseBaudrate(parameters[1]);
                    int databits = ParseDatabits(parameters[2]);
                    Parity parity = ParseParity(parameters[3]);
                    StopBits stopBits = ParseStopBits(parameters[4]);

                    return
                    new SerialPortInterface.SerialPortPhysicalInterface(new SerialPortWrapper(portname, baudrate, parity, databits,
                        stopBits), logger);

                }
                catch (FormatException)
                {
                    
                    return new NullPhysicalInterface(logger);
                }                                
            }

            return new NullPhysicalInterface(logger);
        }

        private StopBits ParseStopBits(string stopBitString)
        {
            if (stopBitString.Equals("n", StringComparison.OrdinalIgnoreCase))
                return StopBits.None;
            if (stopBitString.Equals("1", StringComparison.OrdinalIgnoreCase))
                return StopBits.One;
            if (stopBitString.Equals("1.5", StringComparison.OrdinalIgnoreCase))
                return StopBits.OnePointFive;
            if (stopBitString.Equals("2", StringComparison.OrdinalIgnoreCase))
                return StopBits.Two;

            throw new FormatException();
        }

        private Parity ParseParity(string parityString)
        {
            if (parityString.Equals("n", StringComparison.OrdinalIgnoreCase))
                return Parity.None;
            if (parityString.Equals("o", StringComparison.OrdinalIgnoreCase))
                return Parity.Odd;
            if (parityString.Equals("e", StringComparison.OrdinalIgnoreCase))
                return Parity.Even;
            if (parityString.Equals("s", StringComparison.OrdinalIgnoreCase))
                return Parity.Space;
            if (parityString.Equals("m", StringComparison.OrdinalIgnoreCase))
                return Parity.Mark;


            throw new FormatException();

        }

        private int ParseDatabits(string dataBits)
        {
            return Int32.Parse(dataBits);
        }

        private int ParseBaudrate(string baudrate)
        {
            return Int32.Parse(baudrate);
        }
    }
}
