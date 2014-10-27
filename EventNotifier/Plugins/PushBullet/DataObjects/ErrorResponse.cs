using EventNotifier.Plugins.PushBullet.Annotations;
using RestSharp.Deserializers;

namespace EventNotifier.Plugins.PushBullet.DataObjects
{
    public class ErrorResponse
    {
        [DeserializeAs(Name = "type")]
        public string ErrorType { get; [UsedImplicitly] set; }
        [DeserializeAs(Name = "message")]
        public string Message { get; [UsedImplicitly] set; }
        [DeserializeAs(Name = "param")]
        public string Parameter { get; [UsedImplicitly] set; }
        [DeserializeAs(Name = "cat")]
        public string Cat { get; [UsedImplicitly] set; }

        public override string ToString()
        {
            return string.Format("Type: {0} Message: {1} Parameter: {2} Kittycat: {3}", ErrorType, Message,
                Parameter, Cat);
        }
    }
}