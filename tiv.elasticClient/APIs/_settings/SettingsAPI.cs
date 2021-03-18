using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SimpleJson;
using tiv.elasticClient.APIs._settings.Models;
using tiv.elasticClient.Exceptions;
using tiv.elasticClient.ExtensionFunctions;

namespace tiv.elasticClient.APIs._settings
{
    public class SettingsAPI
    {
        public string Get(IRestClient client, string index, out string resourceCall, int shards = -1, int replicas = -1)
        {
            index = index.TrimSlashes();

            var resource = $"{index}/_settings";

            resourceCall = $"{client.BaseUrl}{resource}";

            var request = new RestRequest(resource, Method.GET) {RequestFormat = DataFormat.Json};
            request.AddHeader("Accept", "application/json");

            var response = client.Execute(request);

            if (response.IsSuccessful) return GetMappingBodyAsString(response.Content, shards, replicas);
            throw new RestCallException(response.StatusCode, response.StatusDescription, response.ErrorMessage, response.ErrorException);
        }

        private string GetMappingBodyAsString(string content, int shards, int replicas)
        {
            var jsonObject = JObject.Parse(content);

            var settingsResponse = JsonConvert.DeserializeObject<SettingsResponse>(jsonObject.First.First.First.First.ToString());

            if (shards > -1)
                settingsResponse.SettingsIndex.NumberOfShards = shards.ToString();

            if (replicas > -1)
                settingsResponse.SettingsIndex.NumberOfReplicas = replicas.ToString();

            // Returning { "index" : ... }
            return JsonConvert.SerializeObject(settingsResponse);
        }
    }
}
