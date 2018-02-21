using RestSharp;
using tiv.elasticClient.APIs.Index.Models;
using tiv.elasticClient.Exceptions;

namespace tiv.elasticClient.APIs.Index
{
    public class IndexAPI
    {
        public DeleteIndexResponse Delete(IRestClient client, string indexOrIndexWildcard, out string resourceCall)
        {
            resourceCall = $"{client.BaseUrl}{indexOrIndexWildcard}";

            var request = new RestRequest(indexOrIndexWildcard, Method.DELETE) { RequestFormat = DataFormat.Json };

            request.AddHeader("Accept", "application/json");
            var response = client.Execute<DeleteIndexResponse>(request);

            if (response.IsSuccessful) return response.Data;
            throw new RestCallException(response.StatusCode, response.StatusDescription, response.ErrorMessage, response.ErrorException);
        }
    }
}
