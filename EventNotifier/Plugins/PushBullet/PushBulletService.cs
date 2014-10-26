using System;
using System.Collections.Generic;
using System.Net;
using Castle.Core.Logging;
using EventNotifier.Plugins.PushBullet.Utils;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace EventNotifier.Plugins.PushBullet
{
    public class PushBulletService : IPushBulletService
    {
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
            request.JsonSerializer = new NewtonJsonSerializer();
            request.AddBody(new PostNoteRequestBody() { PushType = "note", Title = title, Body = body });
            
            var response = restProxy.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                JsonDeserializer deserializer = new JsonDeserializer();
                var errorResponse = deserializer.Deserialize<Dictionary<string, Error>>(response);
                logger.Error("Error posting note to PushBullet");
                logger.ErrorFormat("HTTP StatusCode: {0}",response.StatusCode);
                logger.ErrorFormat("Response: {0}", errorResponse["error"]);
            }
            else
            {
                JsonDeserializer deserializer = new JsonDeserializer();
                PostNoteResponse sucessResponse = deserializer.Deserialize<PostNoteResponse>(response);
            }
            

        }

        public void Initialize(string accessToken, string pushBulletUri)
        {
            restProxy = new RestProxy(accessToken, pushBulletUri);
        }

        private class PostNoteRequestBody
        {
            [SerializeAs(Name = "type")]
            public string PushType { get; set; }
            [SerializeAs(Name = "title")]
            public string Title { get; set; }
            [SerializeAs(Name = "body")]
            public string Body { get; set; }
        }


        
        internal class Error
        {
            [DeserializeAs(Name = "type")]            
            public string ErrorType { get; set; }
            [DeserializeAs(Name = "message")]
            public string Message { get; set; }
            [DeserializeAs(Name = "param")]
            public string Parameter { get; set; }
            [DeserializeAs(Name = "cat")]
            public string Cat { get; set; }

            public override string ToString()
            {
                return string.Format("Type: {0} Message: {1} Parameter: {2} Kittycat: {3}", ErrorType, Message,
                    Parameter, Cat);
            }
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
        }


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

    public class RestProxy
    {
        private readonly string accessToken;
        private readonly string pushBulletUri;

        public RestProxy(string accessToken, string pushBulletUri)
        {
            this.accessToken = accessToken;
            this.pushBulletUri = pushBulletUri;
        }

        public IRestResponse Execute(RestRequest request)
        {
            var client = new RestClient();
            client.BaseUrl = pushBulletUri;
            client.Authenticator = new HttpBasicAuthenticator(accessToken, "");

            JsonDeserializer a = new JsonDeserializer();
            var rawResponse = client.Execute(request);

            var response = client.Execute(request);            
            if (response.ErrorException != null)
            {
                const string message = "error retrieving response.  Check inner details for more info.";
                var ex = new ApplicationException(message, response.ErrorException);
                throw ex;
            }
            return response;
        }
    }
}
