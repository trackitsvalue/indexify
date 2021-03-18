using Newtonsoft.Json;

namespace tiv.elasticClient.APIs._settings.Models
{
    public class SettingsResponse
    {
        [JsonProperty("index")]
        public SettingsIndex SettingsIndex { get; set; }
    }

    public class SettingsIndex
    {
        [JsonProperty("number_of_shards")]
        public string NumberOfShards { get; set; }

        [JsonProperty("number_of_replicas")]
        public string NumberOfReplicas { get; set; }
    }
}
