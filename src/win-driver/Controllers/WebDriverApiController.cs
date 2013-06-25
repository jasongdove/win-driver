using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using WinDriver.ActionFilters;
using WinDriver.Extensions;

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

        protected object Success(Guid sessionId, object value)
        {
            return CreateResponse(sessionId, StatusSuccess, value);
        }

        protected HttpResponseMessage Invalid(object request, InvalidRequest reason)
        {
            var message = new HttpResponseMessage();

            switch (reason)
            {
                case InvalidRequest.UnimplementedCommand:
                    message.StatusCode = HttpStatusCode.NotImplemented;
                    break;
                case InvalidRequest.InvalidCommandMethod:
                    message.StatusCode = HttpStatusCode.MethodNotAllowed;
                    break;
                case InvalidRequest.MissingCommandParameter:
                    message.StatusCode = HttpStatusCode.BadRequest;
                    break;
                default:
                    message.StatusCode = HttpStatusCode.NotFound;
                    break;
            }

            message.Content = new StringContent(
                String.Format("{0} - {1}", reason.Description<InvalidRequest>(), JsonConvert.SerializeObject(request)),
                Encoding.UTF8);

            return message;
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