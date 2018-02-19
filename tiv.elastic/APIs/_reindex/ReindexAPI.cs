using RestSharp;
using tiv.elastic.APIs._reindex.Models;
using tiv.elastic.Exceptions;

namespace tiv.elastic.APIs._reindex
{
    public class ReindexAPI
    {
        public ReindexResponse Post(IRestClient client, ReindexRequest reindexRequest, out string resourceCall)
        {
            var resource = "_reindex";

            resourceCall = $"{client.BaseUrl}{resource}";

            var request = new RestRequest(resource, Method.POST) { RequestFormat = DataFormat.Json };
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(reindexRequest);

            var response = client.Execute<ReindexResponse>(request);

            if (response.IsSuccessful) return response.Data;
            throw new RESTCallException(response.StatusCode, response.StatusDescription, response.ErrorMessage, response.ErrorException);
        } 
    }
}
