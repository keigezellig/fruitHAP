using System;
using Microsoft.Practices.Prism.PubSubEvents;
using FruitHAP.Core.SensorEventPublisher;

namespace FruitHAP.Core.Sensor.SensorTypes
{
    public interface IButton
    {        
		void PressButton();
    }


}
