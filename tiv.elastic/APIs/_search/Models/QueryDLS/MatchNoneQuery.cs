using tiv.elastic.APIs._search.Interfaces;
using tiv.elastic.APIs._search.Models.QueryDLS.components;

namespace tiv.elastic.APIs._search.Models.QueryDLS
{
    public class MatchNoneQuery : IQueryDSL
    {
        public QueryBlankParam match_none { get; set; } = new QueryBlankParam();
    }
}
