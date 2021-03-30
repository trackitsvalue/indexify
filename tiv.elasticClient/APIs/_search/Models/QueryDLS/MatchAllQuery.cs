using tiv.elasticClient.APIs._search.Interfaces;
using tiv.elasticClient.APIs._search.Models.QueryDLS.components;

namespace tiv.elasticClient.APIs._search.Models.QueryDLS
{
    public class MatchAllQuery : IQueryDSL
    {
        public QueryBoostParam match_all { get; set; } = new QueryBoostParam();
    }
}
