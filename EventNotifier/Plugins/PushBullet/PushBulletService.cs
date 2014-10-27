﻿using System.Collections.Generic;
using System.Net;
using Castle.Core.Logging;
using EventNotifier.Plugins.PushBullet.Annotations;
using EventNotifier.Plugins.PushBullet.DataObjects;
using EventNotifier.Plugins.PushBullet.Utils;
using RestSharp;
using RestSharp.Deserializers;

namespace EventNotifier.Plugins.PushBullet
{
    [UsedImplicitly]
    public class PushBulletService : IPushBulletService
    {
        private readonly ILogger logger;
        private RestProxy restProxy;

        public PushBulletService(ILogger logger)
        {
            this.logger = logger ?? NullLogger.Instance;
        }

        public void Initialize(string accessToken, string pushBulletUri)
        {
            restProxy = new RestProxy(accessToken, pushBulletUri);
        }

        public void PostNote(string title, string body)
        {
            var request = CreateNoteRequest(title, body);

            var response = restProxy.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                HandleErrorResponse(response);
            }
            else
            {
                logger.Info("Note successfully posted");
            }

        }

        private void HandleErrorResponse(IRestResponse response)
        {
            JsonDeserializer deserializer = new JsonDeserializer();
            var errorResponse = deserializer.Deserialize<Dictionary<string, ErrorResponse>>(response);
            logger.Error("Error posting note to PushBullet");
            logger.ErrorFormat("HTTP StatusCode: {0}", response.StatusCode);
            logger.ErrorFormat("Response: {0}", errorResponse["error"]);
        }

        private RestRequest CreateNoteRequest(string title, string body)
        {
            RestRequest request = new RestRequest(Method.POST);
            request.Resource = "/v2/pushes";
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new NewtonJsonSerializer();
            request.AddBody(new PostNoteRequest {PushType = "note", Title = title, Body = body});
            return request;
        }
        
    }
}