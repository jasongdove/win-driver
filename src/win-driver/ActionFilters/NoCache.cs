using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace WinDriver.ActionFilters
{
    public class NoCache : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }
    }
}