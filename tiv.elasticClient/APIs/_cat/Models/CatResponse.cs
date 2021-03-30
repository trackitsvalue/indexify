using RestSharp.Deserializers;

namespace tiv.elasticClient.APIs._cat.Models
{
    public class CatResponse
    {
        public string health { get; set; }
        public string status { get; set; }
        public string index { get; set; }
        public string pri { get; set; }
        public string rep { get; set; }

        // Can't get because have dots in their name. Otherwise this would be some sweet info.

        [DeserializeAs(Name = "docs.count")]
        public int docscount { get; set; }

        [DeserializeAs(Name = "docs.deleted")]
        public int docsdeleted { get; set; }

        [DeserializeAs(Name = "store.size")]
        public string storesize { get; set; }

        [DeserializeAs(Name = "pri.store.size")]
        public string pristoresize { get; set; }
    }
}
