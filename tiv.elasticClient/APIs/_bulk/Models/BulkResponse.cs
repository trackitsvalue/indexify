using System.Collections.Generic;
using tiv.elasticClient.GeneralModels;

namespace tiv.elasticClient.APIs._bulk.Models
{
    public class BulkResponse
    {
        public int took { get; set; }
        public bool errors { get; set; }
        public List<BulkCommandOutcome> items { get; set; }
    }

    public class BulkCommandOutcome
    {
        public BulkCommandDocumentInfo create { get; set; }
        public BulkCommandDocumentInfo index { get; set; }
        public BulkCommandDocumentInfo delete { get; set; }
        public BulkCommandDocumentInfo update { get; set; }
    }

    public class BulkCommandDocumentInfo
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public ShardInfo _shards { get; set; }
        public int status { get; set; }
        public int _seq_no { get; set; }
        public int _primary_term { get; set; }
    }
}
