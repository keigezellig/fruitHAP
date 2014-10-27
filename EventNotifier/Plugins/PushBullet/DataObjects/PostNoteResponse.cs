using System;
using RestSharp.Deserializers;

namespace EventNotifier.Plugins.PushBullet.DataObjects
{
    public class PostNoteResponse
    {
        [DeserializeAs(Name = "iden")]
        public string Identity { get; set; }

        [DeserializeAs(Name = "receiver_iden")]
        public string ReceiverIdentity { get; set; }

        [DeserializeAs(Name = "sender_email_normalized")]
        public string SenderEmailNormalized { get; set; }

        [DeserializeAs(Name = "sender_email")]
        public string SenderEmail { get; set; }

        [DeserializeAs(Name = "receiver_email_normalized")]
        public string ReceiverEmailNormalized { get; set; }

        [DeserializeAs(Name = "receiver_email")]
        public string ReceiverEmail { get; set; }

        public string Type { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Active { get; set; }
        public bool Dismissed { get; set; }
            
        [DeserializeAs(Name = "created")]
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}