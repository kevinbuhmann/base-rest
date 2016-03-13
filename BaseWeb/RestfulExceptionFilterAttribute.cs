using General;
using System.Net.Http;
using System.Web.Http.Filters;

namespace BaseWeb
{
    public class RestfulExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            RestfulException restfulException = context.Exception as RestfulException;
            if (restfulException != null)
            {
                context.Response = string.IsNullOrEmpty(restfulException.Message) ?
                    context.Request.CreateResponse(restfulException.StatusCode) :
                    context.Request.CreateResponse(restfulException.StatusCode, new { Message = restfulException.Message });
            }

            base.OnException(context);
        }
    }
}
