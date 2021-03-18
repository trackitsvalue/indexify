using Newtonsoft.Json.Linq;
using RestSharp;
using tiv.elasticClient.Exceptions;
using tiv.elasticClient.ExtensionFunctions;

namespace tiv.elasticClient.APIs._mapping
{
    public class MappingAPI
    {
        public string Get(IRestClient client, string index, out string resourceCall)
        {
            index = index.TrimSlashes();

            var resource = $"{index}/_mapping";

            resourceCall = $"{client.BaseUrl}{resource}";

            var request = new RestRequest(resource, Method.GET) {RequestFormat = DataFormat.Json};
            request.AddHeader("Accept", "application/json");

            var response = client.Execute(request);

            if (response.IsSuccessful) return GetMappingBodyAsString(response.Content);
            throw new RestCallException(response.StatusCode, response.StatusDescription, response.ErrorMessage, response.ErrorException);
        }

        private string GetMappingBodyAsString(string content)
        {
            var jsonObject = JObject.Parse(content);

            // Returning { "properties" : ... }
            return jsonObject.First.First.First.First.ToString();
        }
    }
}
