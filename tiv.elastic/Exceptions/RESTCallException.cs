using System;
using System.Net;

namespace tiv.elastic.Exceptions
{
    public class RESTCallException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string StatusDescription { get; private set; }

        public RESTCallException(HttpStatusCode statusCode, string statusDescription, string errorMessage = "", Exception innerException = null) : base(errorMessage, innerException)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }
    }
}
