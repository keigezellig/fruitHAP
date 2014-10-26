using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp.Serializers;

namespace EventNotifier.Plugins.PushBullet.Utils
{
    public class PropertyNameContractResolver : DefaultContractResolver
    {
        public PropertyNameContractResolver()
            : base(true)
        {
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var result = base.CreateProperty(member, memberSerialization);

            var serializeAsAttribute =
                member.CustomAttributes.SingleOrDefault(f => f.AttributeType == typeof (SerializeAsAttribute));

            if (serializeAsAttribute != null && serializeAsAttribute.NamedArguments != null)
            {
                var nameArgument = serializeAsAttribute.NamedArguments.SingleOrDefault(f => f.MemberName == "Name");
                result.PropertyName = nameArgument.TypedValue.Value.ToString();
            }
            else
            {
                result.PropertyName = member.Name;
            }

            return result;
        }
    }
}
