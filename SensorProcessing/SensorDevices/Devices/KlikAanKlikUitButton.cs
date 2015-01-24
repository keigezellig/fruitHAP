using System;
using System.Security.Cryptography;
using System.Security.Principal;
using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using SensorProcessing.Common.Device;
using SensorProcessing.Common.Eventing;
using SensorProcessing.Common.Pdu;

namespace SensorProcessing.SensorDevices
{
    public class KlikAanKlikUitButton : IButton
    {
        private readonly IEventAggregator eventAggregator;
        private readonly ILogger logger;
        public string Name { get; private set; }
        public string Description { get; private set; }
        public event EventHandler ButtonPressed;

        public KlikAanKlikUitButton(IEventAggregator eventAggregator, ILogger logger)
        {
            this.eventAggregator = eventAggregator;
            this.logger = logger;
        }

        public void Initialize(string name, string description, uint deviceId, byte unitCode, AcCommand command)
        {
            logger.InfoFormat("Initializing button {0}, {1}, {2:X} ({2}), {3:x}, {4}", name,description,deviceId,unitCode,command);
            Name = name;
            Description = description;
            this.eventAggregator.GetEvent<AcPduAvailable>()
                .Subscribe(HandleAcEvent);//, ThreadOption.PublisherThread, false, f => f.DeviceId == deviceId && f.UnitCode == unitCode && f.Command == command);
        }

        private void HandleAcEvent(AcPdu obj)
        {
            logger.DebugFormat("Received event: {0}",obj);
            OnButtonPressed();
        }

        protected virtual void OnButtonPressed()
        {
            if (ButtonPressed != null)
            {
                var localEvent = ButtonPressed;
                localEvent(this, null);
            }
        }
    }
}
