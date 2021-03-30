using RestSharp;
using tiv.elasticClient.APIs._search.Interfaces;
using tiv.elasticClient.APIs._search.Models;
using tiv.elasticClient.Exceptions;

namespace tiv.elasticClient.APIs._search
{
    /// <summary>
    /// More information about this API can be found in Elastic's documenation at:
    /// 
    /// https://www.elastic.co/guide/en/elasticsearch/reference/current/search-request-scroll.html
    /// 
    /// </summary>
    public class SearchAndScrollAPI
    {
        /// <summary>
        /// Initial search POST for a search and scroll request. Will return the initial hits representing the search and will also return the
        /// scroll_id to be used for subsequent PostScroll requests.
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="query"></param>
        /// <param name="indexOrIndexWildcard"></param>
        /// <param name="scrollLifetime"></param>
        /// <param name="resourceCall"></param>
        /// <returns></returns>
        public SearchAndScrollResponse PostSearch(IRestClient client, IRequestBodySearch query, string indexOrIndexWildcard, string scrollLifetime, out string resourceCall)
        {
            var resource = $"{indexOrIndexWildcard}/_search?scroll={scrollLifetime}";
            resourceCall = $"{client.BaseUrl}{resource}";

            var request = new RestRequest(resource, Method.POST) { RequestFormat = DataFormat.Json };
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(query);

            var response = client.Execute<SearchAndScrollResponse>(request);

            if (response.IsSuccessful) return response.Data;
            throw new RestCallException(response.StatusCode, response.StatusDescription, response.ErrorMessage, response.ErrorException);
        }

        /// <summary>
        /// Subsequent requests to scroll for more data from a search and scroll request can be run with this method.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="scrollLifetime"></param>
        /// <param name="scrollId"></param>
        /// <param name="resourceCall"></param>
        /// <returns></returns>
        public SearchAndScrollResponse PostScroll(IRestClient client, string scrollLifetime, string scrollId, out string resourceCall)
        {
            var resource = "_search/scroll";
            resourceCall = $"{client.BaseUrl}{resource}";

            var request = new RestRequest(resource, Method.POST) { RequestFormat = DataFormat.Json };
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(new ScrollRequest() { scroll = scrollLifetime, scroll_id = scrollId});

            var response = client.Execute<SearchAndScrollResponse>(request);

            if (response.IsSuccessful) return response.Data;
            throw new RestCallException(response.StatusCode, response.StatusDescription, response.ErrorMessage, response.ErrorException);
        }
    }
}
