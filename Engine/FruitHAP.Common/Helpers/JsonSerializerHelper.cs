using System.IO;
using Newtonsoft.Json;

namespace FruitHAP.Common.Helpers
{
    public static class JsonSerializerHelper
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

		public static T ParseJsonString<T>(this string input) where T: class
		{
			var result = default(T);
			using (StringReader sr = new StringReader(input))
			{
				using (JsonReader tr = new JsonTextReader(sr))
				{
					Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
					result = serializer.Deserialize<T>(tr);
				}
			}
			return result;
		}

		public static string ToJsonString<T>(this T obj) where T: class
		{
			string result = "";
			using (StringWriter sw = new StringWriter())
			{
				using (JsonTextWriter tw = new JsonTextWriter(sw))
				{
					JsonSerializer serializer = new JsonSerializer();
					serializer.Serialize(tw,obj);                    
				}
				result = sw.ToString ();
			}

			return result;
		}
    }
}
