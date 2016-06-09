using System;

namespace FruitHAP.Sensor.FruitSensor
{
    public class RfxFruitProtocol
    {
        public RfxFruitValue Decode(uint valueToBeDecoded)
        {
            RfxFruitValue result;
            uint isSignBitSet = valueToBeDecoded & 0x80000;
            uint type = valueToBeDecoded >> 20;
            uint value = valueToBeDecoded & 0x7FFFF;

            long endvalue = isSignBitSet != 0 ? -value : value;

            result.Quantity = (RfxFruitQuantity)type;
            result.Value = endvalue;

            return result;
        }

    }

    public struct RfxFruitValue
    {
        public long Value {get; set;}
        public RfxFruitQuantity Quantity { get; set; }
    };


    public enum RfxFruitQuantity
    {
        Unknown = 0x00,
        TemperatureInCentiCelsius = 0x01,
        HumidityInPercentage = 0x02
    }
}

