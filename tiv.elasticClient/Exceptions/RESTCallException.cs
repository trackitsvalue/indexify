using System;
using System.Net;

namespace tiv.elasticClient.Exceptions
{
    public class RestCallException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string StatusDescription { get; private set; }

        public RestCallException(HttpStatusCode statusCode, string statusDescription, string errorMessage = "", Exception innerException = null) : base(errorMessage, innerException)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }
    }
}
