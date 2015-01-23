using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorProcessing.Common.Device
{
    public interface IButton : IDevice
    {
        event EventHandler ButtonPressed;
    }
}
