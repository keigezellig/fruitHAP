using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp.Serializers;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace EventNotifier.Plugins.PushBullet.Utils
{
    public class NewtonJsonSerializer : ISerializer
    {
        private readonly JsonSerializer serializer;
        public string ContentType { get; set; }
        public string DateFormat { get; set; }
        public string Namespace { get; set; }
        public string RootElement { get; set; }

        public NewtonJsonSerializer()
        {
            ContentType = "application/json";
            serializer = new JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,

            };
        }

        public NewtonJsonSerializer(JsonSerializer serializer)
	        {
	            ContentType = "application/json";
	            this.serializer = serializer;
	        }

        public string Serialize(object obj)
        {
            string json =
                JsonConvert.SerializeObject(
                    obj,
                    Formatting.Indented,
                    new JsonSerializerSettings { ContractResolver = new PropertyNameContractResolver() }
                    );
            return json;
        }
    }
}
