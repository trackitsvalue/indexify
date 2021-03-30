using tiv.elasticClient.APIs._search.Interfaces;

namespace tiv.elasticClient.APIs._search.Models
{
    public class SearchAndScrollRequest : IRequestBodySearch
    {
        // public int? from { get; set; }
        public int? size { get; set; }
        public IQueryDSL query { get; set; }
    }
}
