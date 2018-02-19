using RestSharp;
using RestSharp.Authenticators;

namespace tiv.elastic
{
    public class RESTClientUtility
    {
        public static RESTClientUtility Instance { get; } = new RESTClientUtility();

        public IRestClient NewClient(string url, IAuthenticator auth)
        {
            var client = new RestClient(url);
            
            if (auth != null)
            {
                client.Authenticator = auth;
            }
            return client;
        }
    }
}
