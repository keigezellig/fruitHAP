using System;

namespace FruitHAP.Sensor.FruitSensor
{
    public class RfxFruitProtocol
    {
        public RfxFruitValue Decode(uint valueToBeDecoded)
        {
            RfxFruitValue result = new RfxFruitValue();
            uint isSignBitSet = valueToBeDecoded & 0x80000;
            uint type = valueToBeDecoded >> 20;
            uint value = valueToBeDecoded & 0x7FFFF;

            long endvalue = isSignBitSet != 0 ? -value : value;

            if (Enum.IsDefined(typeof(RfxFruitQuantity), (Int32)type))
            {
                result.Quantity = (RfxFruitQuantity)type;    
            }
            else
            {
                result.Quantity = RfxFruitQuantity.Unknown;
            }
            
            result.Value = endvalue;

            return result;
        }

    }

    public struct RfxFruitValue
    {
        public long Value {get; set;}
        public RfxFruitQuantity Quantity { get; set; }
    }


    public enum RfxFruitQuantity
    {
        Unknown = 0xFF,
        TemperatureInCentiCelsius = 0x00,
        HumidityInPercentage = 0x01
    }
}

