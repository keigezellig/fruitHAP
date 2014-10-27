using System;
using RestSharp;

namespace EventNotifier.Plugins.PushBullet
{
    public class RestProxy
    {
        private readonly string accessToken;
        private readonly string uri;

        public RestProxy(string accessToken, string uri)
        {
            this.accessToken = accessToken;
            this.uri = uri;
        }

        public IRestResponse Execute(RestRequest request)
        {
            var client = new RestClient {BaseUrl = uri, Authenticator = new HttpBasicAuthenticator(accessToken, "")};

            var response = client.Execute(request);
            
            if (response.ErrorException == null)
            {
                return response;
            }
            
            throw new ApplicationException("Error retrieving response.  Check inner details for more info.", response.ErrorException);
        }
    }
}