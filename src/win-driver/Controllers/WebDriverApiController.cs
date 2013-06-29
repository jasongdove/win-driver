using System;
using System.Net.Http;
using System.Web.Http;
using Ninject;
using WinDriver.Filters;
using WinDriver.Repository;

namespace WinDriver.Controllers
{
    [NoCache]
    [SessionExceptionFilter]
    public abstract class WebDriverApiController : ApiController
    {
        private const int StatusSuccess = 0;

        [Inject]
        private IResponseRepository ResponseRepository { get; set; }

        protected object Success(object value)
        {
            return CreateResponse(null, StatusSuccess, value);
        }

        protected object Success(Guid sessionId, object value)
        {
            return CreateResponse(sessionId, StatusSuccess, value);
        }

        protected HttpResponseMessage Invalid(object request, InvalidRequest reason)
        {
            return ResponseRepository.Invalid(request, reason);
        }

        private object CreateResponse(Guid? sessionId, int status, object value)
        {
            return new
            {
                sessionId = sessionId.HasValue ? sessionId.Value.ToString("N") : null,
                status,
                value
            };
        }
    }
}