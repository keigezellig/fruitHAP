using Castle.Core.Logging;
using FakeItEasy;
using NUnit.Framework;
using SensorProcessing.Common.InterfaceReaders;
using SensorProcessing.Common.InterfaceReaders.SerialPort;

namespace SensorBinding.Tests
{
    [TestFixture]
    public class InterfaceReaderFactoryTest
    {
        [Test]
        public void CreateSerialInterfaceReaderWithCorrectConnectionString()
        {
            var fakeLogger = A.Fake<ILogger>();
            string connectionString = "serial:COM1,38400,8,N,1";
            var factory = new InterfaceReaderFactory(fakeLogger);
            var reader = factory.CreateInterfaceReader(connectionString);
            Assert.IsNotNull(reader);
            Assert.IsInstanceOf(typeof(SerialPortInterfaceReader),reader);
        }

        [Test]
        public void CreateInterfaceReaderWithEmptyConnectionString()
        {
            var fakeLogger = A.Fake<ILogger>();
            string connectionString = "";
            var factory = new InterfaceReaderFactory(fakeLogger);
            var reader = factory.CreateInterfaceReader(connectionString);
            Assert.IsNotNull(reader);
            Assert.IsInstanceOf(typeof(NullInterfaceReader), reader);
            
        }

        [Test]
        public void CreateInterfaceReaderWithUnsupportedType()
        {
            var fakeLogger = A.Fake<ILogger>();
            string connectionString = "tcp:192.167.1.3:444";
            var factory = new InterfaceReaderFactory(fakeLogger);
            var reader = factory.CreateInterfaceReader(connectionString);
            Assert.IsNotNull(reader);
            Assert.IsInstanceOf(typeof(NullInterfaceReader), reader);
        }

        [Test]
        public void CreateSerialInterfaceReaderWithMalformedConnectionString()
        {
            var fakeLogger = A.Fake<ILogger>();
            string connectionString = "serial:asdasd,wiw,d";
            var factory = new InterfaceReaderFactory(fakeLogger);
            var reader = factory.CreateInterfaceReader(connectionString);
            Assert.IsNotNull(reader);
            Assert.IsInstanceOf(typeof(NullInterfaceReader), reader);
        }
        [Test]
        public void CreateSerialInterfaceReaderWithOtherStopBits()
        {
            var fakeLogger = A.Fake<ILogger>();
            string connectionString = "serial:COM1,38400,8,N,2";
            var factory = new InterfaceReaderFactory(fakeLogger);
            var reader = factory.CreateInterfaceReader(connectionString);
            Assert.IsNotNull(reader);
            Assert.IsInstanceOf(typeof(SerialPortInterfaceReader), reader);
        }





    }
}
