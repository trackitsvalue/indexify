using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using tiv.elasticClient.APIs.Index.Models;
using tiv.elasticClient.Exceptions;

namespace tiv.elasticClient.APIs.Index
{
    public class IndexAPI
    {
        public IndexResponse Create(IRestClient client, string index, string settings, string mappings, out string resourceCall)
        {
            resourceCall = $"{client.BaseUrl}{index}";

            var request = new RestRequest(index, Method.PUT) { RequestFormat = DataFormat.Json };

            request.AddHeader("Accept", "application/json");

            request.AddParameter(
               "application/json",
               $@"{{""settings"": {settings}, ""mappings"": {mappings}}}",
               ParameterType.RequestBody);

            var response = client.Execute<IndexResponse>(request);

            if (response.IsSuccessful) return response.Data;
            throw new RestCallException(response.StatusCode, response.StatusDescription, response.ErrorMessage, response.ErrorException);
        }

        public IndexResponse Delete(IRestClient client, string indexOrIndexWildcard, out string resourceCall)
        {
            resourceCall = $"{client.BaseUrl}{indexOrIndexWildcard}";

            var request = new RestRequest(indexOrIndexWildcard, Method.DELETE) { RequestFormat = DataFormat.Json };

            request.AddHeader("Accept", "application/json");
            var response = client.Execute<IndexResponse>(request);

            if (response.IsSuccessful) return response.Data;
            throw new RestCallException(response.StatusCode, response.StatusDescription, response.ErrorMessage, response.ErrorException);
        }
    }
}
