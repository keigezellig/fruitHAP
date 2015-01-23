using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using Microsoft.Practices.Prism.PubSubEvents;
using SensorProcessing.Common;
using SensorProcessing.Common.Device;
using SensorProcessing.Common.Pdu;
using SensorProcessing.SensorDevices.Devices;

namespace SensorProcessing.SensorDevices
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly ILogger logger;
        private readonly IEventAggregator eventAggregator;
        private List<IDevice> devices; 

        public DeviceRepository(ILogger logger, IEventAggregator eventAggregator)
        {
            this.logger = logger;
            this.eventAggregator = eventAggregator;
        }

        public void LoadDevices()
        {
            devices = new List<IDevice>();
            
            var button = new KlikAanKlikUitButton(eventAggregator, logger);
            button.Initialize("Doorbell","My doorbell",0x00E41822,01,AcCommand.GroupOn);

            var ipcam = new IpCamera(logger);
            ipcam.Initialize("DoorCamera","My front door camera",new Uri("http://bihac/snapshot.cgi"),"visitor","visitor");

            devices.Add(button);
            devices.Add(ipcam);
        }


        public List<T> FindAllDevicesOfType<T>() where T : IDevice
        {
            return devices.OfType<T>().ToList();
        }

        public T FindDeviceOfTypeByName<T>(string name) where T : IDevice
        {
            return devices.OfType<T>().SingleOrDefault(f => f.Name == name);
        }
    }
}
