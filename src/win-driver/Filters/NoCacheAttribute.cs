using System.Net.Http.Headers;
using System.Web.Http.Filters;

namespace WinDriver.Filters
{
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }
    }
}