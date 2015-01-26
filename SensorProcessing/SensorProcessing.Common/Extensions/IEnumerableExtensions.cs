using System.Collections.Generic;
using System.Text;

namespace FruitHAP.SensorProcessing.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static string PrettyPrint(this IEnumerable<byte> byteArray)
        {
            StringBuilder sb = new StringBuilder();

            foreach(var b in byteArray)
            {
                sb.AppendFormat("{0:X} ", b);
            }
            return sb.ToString();

        }
    }
}
