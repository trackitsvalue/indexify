namespace tiv.elasticClient.APIs._reindex.Models
{
    public class ReindexRequest
    {
        public string conflicts { get; set; }
        public ReindexSource source { get; set; }
        public ReindexDestination dest { get; set; }
    }

    public class ReindexSource
    {
        public string index { get; set; }
    }

    public class ReindexDestination
    {
        public string index { get; set; }
        public string op_type { get; set; }
    }
}
