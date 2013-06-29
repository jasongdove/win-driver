using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using WinDriver.Extensions;

namespace WinDriver.Repository
{
    public class ResponseRepository : IResponseRepository
    {
        public HttpResponseMessage Invalid(object request, InvalidRequest reason)
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
    }
}