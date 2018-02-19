using RestSharp;
using tiv.elastic.APIs._bulk.Models;
using tiv.elastic.Exceptions;

namespace tiv.elastic.APIs._bulk
{
    /// <summary>
    /// More information about this API can be found in Elastic's documenation at:
    /// 
    /// https://www.elastic.co/guide/en/elasticsearch/reference/current/docs-bulk.html
    /// 
    /// </summary>
    public class BulkAPI
    {
        /// <summary>
        /// Perform a bulk index opration as defined by the passed bulkData parameter. See BulkExtensionMethods.cs for an easy way to define bulk operations
        /// based upon Search and Scroll results.
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="bulkRequest"></param>
        /// <param name="resourceCall"></param>
        /// <returns></returns>
        public BulkResponse Post(IRestClient client, string bulkRequest, out string resourceCall)
        {
            var resource = "_bulk";
            resourceCall = $"{client.BaseUrl}{resource}";

            var request = new RestRequest(resource, Method.POST) { RequestFormat = DataFormat.Json };
            request.AddHeader("Accept", "application/json");
            request.AddParameter("text/ndjson", bulkRequest, ParameterType.RequestBody);
//            request.AddJsonBody(bulkRequest);

            var response = client.Execute<BulkResponse>(request);

            if (response.IsSuccessful) return response.Data;
            throw new RESTCallException(response.StatusCode, response.StatusDescription, response.ErrorMessage, response.ErrorException);
        }
    }
}
