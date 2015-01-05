using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SensorBinding.Decoders.RfxDecoder;
using SensorBinding.Pdu;

namespace SensorBinding.Tests
{
    [TestClass]
    public class DecoderTest
    {
        [TestMethod]
        public void CanDecodeAnAcInputByteArray()
        {
            AcPdu result = null;
            byte[] input = new byte[] {0x0B,0x11,0x00,0x0E,0x00,0x00,0x00,0x08,0x01,0x05,0x06,0x00};
            RfxDecoder decoder = new RfxDecoder();
            decoder.Decode(input);

            Assert.AreEqual(14, RfxDecoder.SequenceNumber);
            Assert.IsNotNull(result);
        }
    }
}
