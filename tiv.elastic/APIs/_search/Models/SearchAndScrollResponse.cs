using tiv.elastic.GeneralModels;

namespace tiv.elastic.APIs._search.Models
{
    public class SearchAndScrollResponse
    {
        public string scroll_id { get; set; }
        public int took { get; set; }
        public bool timed_out { get; set; }
        public bool terminated_early { get; set; }
        public ShardInfo _shards { get; set; }
        public SearchHits hits { get; set; }
    }
}
