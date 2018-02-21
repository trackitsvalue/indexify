using RestSharp;
using RestSharp.Authenticators;

namespace tiv.elasticClient
{
    public class RestClientUtility
    {
        public static RestClientUtility Instance { get; } = new RestClientUtility();

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
