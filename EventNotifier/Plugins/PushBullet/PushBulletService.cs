using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using RestSharp;
using RestSharp.Deserializers;

namespace EventNotifier.Plugins.PushBullet
{
    public class PushBulletService : IPushBulletService
    {
        private string accessToken;
        private ILogger logger;
        private RestProxy restProxy;

        public PushBulletService(ILogger logger)
        {
            this.logger = logger ?? NullLogger.Instance;
        }

        public void PostNote(string title, string body)
        {
            RestRequest request = new RestRequest(Method.POST);
            request.Resource = "/v2/pushes";
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new PostNoteRequestBody() {Type = "Note", Title = title, Body = body});
            var result = restProxy.Execute<PostNoteResponse>(request);

        }

        public void Initialize(string accessToken, string pushBulletUri)
        {
            restProxy = new RestProxy(accessToken, pushBulletUri);
        }



        private class PostNoteRequestBody
        {
            public string Type { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
        }

        internal class PostNoteResponse
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
            public DateTime Created { get; set; }
            public DateTime Modified { get; set; }



            /*{
  ""iden": "ujxvzW0RqaypjArHrh8WLA",
    "sender_email_normalized": "mhjoosten666@gmail.com",
    "receiver_iden": "ujxvzW0Rqay",
    "receiver_email_normalized": "mhjoosten666@gmail.com",
    "title": "\"hello\"",
    "dismissed": false,
    "receiver_email": "mh.joosten666@gmail.com",
    "type": "note",
    "body": "\"Hi there!",
    "active": true,
    "sender_iden": "ujxvzW0Rqay",
    "created": 1406664511.99769,
    "modified": 1406664511.9977,
    "sender_email": "mh.joosten666@gmail.com"
}*/
        }
    }

    public class RestProxy
    {
        private readonly string accessToken;
        private readonly string pushBulletUri;

        public RestProxy(string accessToken, string pushBulletUri)
        {
            this.accessToken = accessToken;
            this.pushBulletUri = pushBulletUri;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var client = new RestClient();
            client.BaseUrl = pushBulletUri;
            client.Authenticator = new HttpBasicAuthenticator(accessToken, "");
            var response = client.Execute<T>(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var ex = new ApplicationException(message, response.ErrorException);
                throw ex;
            }
            return response.Data;
        }
    }
}
