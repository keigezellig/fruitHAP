using EventNotifier.Plugins.PushBullet.Annotations;
using RestSharp.Serializers;

namespace EventNotifier.Plugins.PushBullet.DataObjects
{
    public class PostNoteRequest
    {
        [SerializeAs(Name = "type")]
        public string PushType { [UsedImplicitly] get; set; }
        [SerializeAs(Name = "title")]
        public string Title { [UsedImplicitly] get; set; }
        [SerializeAs(Name = "body")]
        public string Body { [UsedImplicitly] get; set; }
        [SerializeAs(Name = "channel_tag")]
        public string Channel { [UsedImplicitly] get; set; }
    }
}