using tiv.elasticClient.APIs._search.Interfaces;
using tiv.elasticClient.APIs._search.Models.QueryDLS.components;

namespace tiv.elasticClient.APIs._search.Models.QueryDLS
{
    public class MatchNoneQuery : IQueryDSL
    {
        public QueryBlankParam match_none { get; set; } = new QueryBlankParam();
    }
}
