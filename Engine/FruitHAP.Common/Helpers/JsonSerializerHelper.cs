using System.IO;
using Newtonsoft.Json;

namespace FruitHAP.Common.Helpers
{
    public class JsonSerializerHelper
    {
        public static void Serialize<T>(string fileName, T obj)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                using (JsonTextWriter tw = new JsonTextWriter(sw))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(tw,obj);                    
                }
            }
        }

        public static T Deserialize<T>(string fileName)
        {
            var result = default(T);
            using (StreamReader sr = new StreamReader(fileName))
            {
                using (JsonReader tr = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    result = serializer.Deserialize<T>(tr);
                }
            }
            return result;
        }
    }
}
