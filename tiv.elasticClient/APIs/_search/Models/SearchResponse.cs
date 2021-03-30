using System.Collections.Generic;
using tiv.elasticClient.GeneralModels;

namespace tiv.elasticClient.APIs._search.Models
{
    public class SearchResponse
    {
        public string scroll_id { get; set; }
        public int took { get; set; }
        public bool timed_out { get; set; }
        public bool terminated_early { get; set; }
        public ShardInfo _shards { get; set; }
        public SearchHits hits { get; set; }
    }

    public class SearchHits
    {
        public int total { get; set; }
        public int max_score { get; set; }
        public List<SearchHit> hits { get; set; }
    }

    public class SearchHit
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public int _score { get; set; }
        public string _source { get; set; }
    }
}
