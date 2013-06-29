using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using WinDriver.Repository;

namespace WinDriver.Controllers
{
    public class SessionController : WebDriverApiController
    {
        private readonly ILogger _logger;
        private readonly ISessionRepository _sessionRepository;

        public SessionController(ILogger logger, ISessionRepository sessionRepository)
        {
            _logger = logger;
            _sessionRepository = sessionRepository;
        }

        [ActionName("DefaultAction")]
        public object Post(Dictionary<string, JObject> parameters)
        {
            if (parameters.ContainsKey("requiredCapabilities"))
            {
                // TODO: verify that required capabilities can be supported, otherwise return session_not_created
            }

            if (parameters.ContainsKey("desiredCapabilities"))
            {
                // TODO: verify that app exists, otherwise return session_not_created

                // TODO: support more desired capabilities, rather than just ignoring them
                var desiredCapabilities = new Capabilities(parameters["desiredCapabilities"]);
                var session = _sessionRepository.Create(desiredCapabilities);

                var response = Request.CreateResponse(HttpStatusCode.RedirectMethod);
                response.Headers.Location = new Uri(Url.Link("DefaultApiWithId", new { controller = "session", id = session.SessionKey }));
                return response;
            }

            return Invalid(parameters, InvalidRequest.MissingCommandParameter);
        }

        [ActionName("DefaultAction")]
        public object Get(string id)
        {
            var session = _sessionRepository.GetById(id);

            return Success(session.SessionId, session.Capabilities);
        }

        public object GetAll()
        {
            var allSessions = _sessionRepository.GetAll().Select(x => new
            {
                id = x.SessionKey,
                capabilities = x.Capabilities
            });

            return Success(allSessions);
        }

        [ActionName("DefaultAction")]
        public object Delete(string id)
        {
            var session = _sessionRepository.GetById(id);
            session.Delete();

            return Success(session.SessionId, null);
        }

        [ActionName("window_handles")]
        public object GetWindowHandles(string id)
        {
            var session = _sessionRepository.GetById(id);
            var handles = session.GetWindowHandles().ToArray();

            return Success(session.SessionId, handles);
        }

        [ActionName("title")]
        public object GetTitle(string id)
        {
            var session = _sessionRepository.GetById(id);
            var title = session.Title;

            return Success(session.SessionId, title);
        }

        [ActionName("window")]
        public object SwitchToWindow(string id, Dictionary<string, JObject> parameters)
        {
            var session = _sessionRepository.GetById(id);
            var name = parameters["name"].Value<string>();

            session.SwitchToWindow(name);

            // TODO: return failure if we aren't able to switch
            return Success(session.SessionId, null);
        }
    }
}