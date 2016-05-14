using System;

namespace FruitHAP.Common.Helpers
{
	public static class BitBangExtensions
	{		
		public static byte SetBit(this byte value, byte bitNo)
		{
			byte newValue = (byte)(value | (1 << bitNo));
			return newValue;
		}
			

		public static byte ClearBit(this byte value, byte bitNo)
		{
			byte newValue = (byte)(value & ~(1 << bitNo));
			return newValue;
		}

		public static byte ToggleBit(this byte value, byte bitNo)
		{
			byte newValue = (byte)(value ^ (1 << bitNo));
			return newValue;
		}
		public static bool IsBitSet(this byte value, byte bitNo)
		{
			return (value & (1 << bitNo)) == 1;
		}




	}
}

