namespace tiv.elasticClient.APIs._reindex.Models
{
    public class ReindexResponse
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public int total { get; set; }
        public int updated { get; set; }
        public int created { get; set; }
        public int deleted { get; set; }
        public int batches { get; set; }
        public int version_conflicts { get; set; }
        public int noops { get; set; }
        //"retries": {
        //    "bulk": 0,
        //    "search": 0
        //},
        public int throttled_millis { get; set; }
        public int requests_per_second { get; set; }
        public int throttled_until_millis { get; set; }
        //"failures": []
    }
}
