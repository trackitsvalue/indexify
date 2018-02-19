using tiv.elastic.APIs._search.Interfaces;

namespace tiv.elastic.APIs._search.Models
{
    public class SearchAndScrollRequest : IRequestBodySearch
    {
        // public int? from { get; set; }
        public int? size { get; set; }
        public IQueryDSL query { get; set; }
    }
}
