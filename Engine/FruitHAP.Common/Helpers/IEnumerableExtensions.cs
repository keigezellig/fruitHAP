using System.Collections.Generic;
using System.Text;

namespace FruitHAP.Common.Helpers
{
    public static class IEnumerableExtensions
    {
        public static string BytesAsString(this IEnumerable<byte> byteArray)
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
