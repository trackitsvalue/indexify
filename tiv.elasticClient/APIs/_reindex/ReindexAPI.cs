using RestSharp;
using tiv.elasticClient.APIs._reindex.Models;
using tiv.elasticClient.Exceptions;

namespace tiv.elasticClient.APIs._reindex
{
    public class ReindexAPI
    {
        public ReindexResponse Post(IRestClient client, ReindexRequest reindexRequest, out string resourceCall)
        {
            var resource = "_reindex";

            resourceCall = $"{client.BaseUrl}{resource}";

            var request = new RestRequest(resource, Method.POST) { RequestFormat = DataFormat.Json };
            request.AddHeader("Accept", "application/json");
            request.AddQueryParameter("slices", "auto");
            request.AddJsonBody(reindexRequest);

            var response = client.Execute<ReindexResponse>(request);

            if (response.IsSuccessful) return response.Data;
            throw new RestCallException(response.StatusCode, response.StatusDescription, response.ErrorMessage, response.ErrorException);
        } 
    }
}
