using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;

namespace WinDriver.Controllers
{
    public class SessionController : WebDriverApiController
    {
        private static readonly MemoryCache SessionCache;
        private readonly ILogger _logger;

        static SessionController()
        {
            SessionCache = new MemoryCache("Sessions");
        }

        public SessionController(ILogger logger)
        {
            _logger = logger;
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
                var session = new Session(new Capabilities(parameters["desiredCapabilities"]));

                SessionCache.Add(
                    session.SessionKey,
                    session,
                    new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(5), RemovedCallback = SessionRemoved });

                _logger.Info(String.Format("New Session Created: {0}", session.SessionKey));

                var response = Request.CreateResponse(HttpStatusCode.RedirectMethod);
                response.Headers.Location = new Uri(Url.Link("DefaultApiWithId", new { controller = "session", id = session.SessionKey }));
                return response;
            }

            return Invalid(parameters, InvalidRequest.MissingCommandParameter);
        }

        [ActionName("DefaultAction")]
        public object Get(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return Invalid(null, InvalidRequest.MissingCommandParameter);
            }

            if (!SessionCache.Contains(id))
            {
                return Invalid(null, InvalidRequest.VariableResourceNotFound);
            }

            var session = (Session)SessionCache[id];

            return Success(session.SessionId, session.Capabilities);
        }

        public object GetAll()
        {
            var allSessions = SessionCache.Select(x => new
            {
                id = ((Session)x.Value).SessionKey,
                capabilities = ((Session)x.Value).Capabilities
            });

            return Success(allSessions);
        }

        [ActionName("DefaultAction")]
        public object Delete(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return Invalid(null, InvalidRequest.MissingCommandParameter);
            }

            if (!SessionCache.Contains(id))
            {
                return Invalid(null, InvalidRequest.VariableResourceNotFound);
            }

            var session = (Session)SessionCache.Remove(id);
            session.Delete();

            return Success(session.SessionId, null);
        }

        [ActionName("window_handles")]
        public object GetWindowHandles(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return Invalid(null, InvalidRequest.MissingCommandParameter);
            }

            if (!SessionCache.Contains(id))
            {
                return Invalid(null, InvalidRequest.VariableResourceNotFound);
            }

            var session = (Session)SessionCache.Get(id);
            var handles = session.GetWindowHandles().ToArray();

            return Success(session.SessionId, handles);
        }

        private void SessionRemoved(CacheEntryRemovedArguments args)
        {
            // give the session an opportunity to clean up when it expires
            if (args.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                var session = (Session)args.CacheItem.Value;
                session.Delete();
            }
        }
    }
}