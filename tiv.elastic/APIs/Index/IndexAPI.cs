using RestSharp;
using tiv.elastic.APIs.Index.Models;
using tiv.elastic.Exceptions;

namespace tiv.elastic.APIs.Index
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
            throw new RESTCallException(response.StatusCode, response.StatusDescription, response.ErrorMessage, response.ErrorException);
        }
    }
}
