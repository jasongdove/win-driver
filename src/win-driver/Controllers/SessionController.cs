using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using Ninject.Extensions.Logging;

namespace WinDriver.Controllers
{
    public class SessionController : WebDriverApiController
    {
        private static readonly ConcurrentDictionary<Guid, Session> Sessions;
        private readonly ILogger _logger;

        static SessionController()
        {
            Sessions = new ConcurrentDictionary<Guid, Session>();
        }

        public SessionController(ILogger logger)
        {
            _logger = logger;
        }

        public object Post(Command command)
        {
            if (command.Parameters.ContainsKey("requiredCapabilities"))
            {
                // TODO: verify that required capabilities can be supported, otherwise return session_not_created
            }

            if (command.Parameters.ContainsKey("desiredCapabilities"))
            {
                // TODO: support desired capabilities, rather than just ignoring them
                var session = new Session();
                Sessions[session.SessionId] = session;

                // TODO: launch application

                _logger.Info(String.Format("New Session Created: {0}", session.SessionId));

                var response = Request.CreateResponse(HttpStatusCode.RedirectMethod);
                response.Headers.Location = new Uri(Url.Link("DefaultApiWithId", new { controller = "session", id = session.SessionId.ToString("N") }));
                return response;
            }

            return Invalid(command, InvalidRequest.MissingCommandParameter);
        }

        public object Get(Guid? id)
        {
            if (!id.HasValue)
            {
                return Invalid(null, InvalidRequest.MissingCommandParameter);
            }

            if (!Sessions.ContainsKey(id.Value))
            {
                return Invalid(null, InvalidRequest.VariableResourceNotFound);
            }

            var session = Sessions[id.Value];

            return Success(session.SessionId, session.Capabilities);
        }
    }
}