using System;
using System.Collections.Generic;
using FakeItEasy;
using NUnit;
using NUnit.Framework;
using SensorProcessing.Common.Pdu;
using SensorProcessing.SensorBinding.RfxBinding;
using SensorProcessing.SensorBinding.RfxBinding.Decoders;
using SensorProcessing.SensorBinding.RfxBinding.Decoders.RfxDecoder;
using SensorProcessing.SensorBinding.RfxBinding.Eventing;

namespace SensorBinding.Tests
{
    [TestFixture]
    public class DecoderTest
    {
        private IAcPduPublisher eventPublisherFake;
        private RfxProtocol protocol;

        [SetUp]
        public void Setup()
        {
            eventPublisherFake = A.Fake<IAcPduPublisher>();
            A.CallTo(() => eventPublisherFake.Publish(A<AcPdu>.Ignored)).Invokes(AssertAcPdu);
            var acDecoder = new AcSubProtocolDecoder(eventPublisherFake);
            var lightingTwoDecoder = new LightingTwoProtocolDecoder(new List<IDecoder>() { acDecoder });

            protocol = new RfxProtocol(new List<IDecoder>() { lightingTwoDecoder });
            
        }

        private void AssertAcPdu(FakeItEasy.Core.IFakeObjectCall obj)
        {
            var pdu = obj.Arguments.Get<AcPdu>(0);
            Assert.AreEqual((uint)0x00E41822, pdu.DeviceId);
            Assert.AreEqual(1, pdu.UnitCode);
            Assert.AreEqual(AcCommand.SetGroupLevel, pdu.Command);
            Assert.AreEqual(6, pdu.Level);
        }

        [Test]
        public void CanDecodeAnAcInputByteArray()
        {
            byte[] input = new byte[] {0x0B,0x11,0x00,0x0E,0x00,0xE4,0x18,0x22,0x01,0x05,0x06,0x00};
            protocol.Process(input);

            A.CallTo(() => eventPublisherFake.Publish(A<AcPdu>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
        }

 
        [Test]
        public void CannotDecodeOtherLighting2Input()
        {
            byte[] input = new byte[] {0x0B,0x11,0x02,0x0E,0x00,0xE4,0x18,0x22,0x01,0x05,0x06,0x00};
            protocol.Process(input);

            A.CallTo(() => eventPublisherFake.Publish(A<AcPdu>.Ignored)).MustNotHaveHappened();
            
        }

        [Test]
        public void CannotDecodeOtherProtocolInput()
        {
            byte[] input = new byte[] { 0x0B, 0x13, 0x02, 0x0E, 0x00, 0xE4, 0x18, 0x22, 0x01, 0x05, 0x06, 0x00 };
            protocol.Process(input);

            A.CallTo(() => eventPublisherFake.Publish(A<AcPdu>.Ignored)).MustNotHaveHappened();

        }
    }
}
