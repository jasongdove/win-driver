using System.Web.Http;
using WinDriver.ActionFilters;

namespace WinDriver.Controllers
{
    [NoCache]
    public abstract class WebDriverApiController : ApiController
    {
        private const int StatusSuccess = 0;

        protected object Success(object value)
        {
            return CreateResponse(null, StatusSuccess, value);
        }

        protected object Success(int sessionId, object value)
        {
            return CreateResponse(sessionId, StatusSuccess, value);
        }

        private object CreateResponse(int? sessionId, int status, object value)
        {
            return new { sessionId, status, value };
        }
    }
}