using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;

namespace General
{
    public class RestfulException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public RestfulException(HttpStatusCode statusCode, string message = "")
            : base(message)
        {
            this.StatusCode = statusCode;
        }

        public IHttpActionResult ToActionResult(ApiController controller)
        {
            return new NegotiatedContentResult<object>(this.StatusCode, new { Message = this.Message }, controller);
        }
    }
}
