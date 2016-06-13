using System;

namespace FruitHAP.Core.Sensor.SensorValueTypes
{
    public class PercentageQuantity : Quantity<String>
    {
        public PercentageQuantity()
        {
            this.Unit = "%";
        }
    }
}

