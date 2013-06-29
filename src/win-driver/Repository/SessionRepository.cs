using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Ninject.Extensions.Logging;
using WinDriver.Exceptions;

namespace WinDriver.Repository
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ILogger _logger;
        private readonly MemoryCache _sessionCache;

        public SessionRepository(ILogger logger)
        {
            _logger = logger;
            _sessionCache = new MemoryCache("Sessions");
        }

        public Session GetById(string sessionId)
        {
            if (String.IsNullOrEmpty(sessionId))
            {
                throw new MissingCommandParameterException();
            }

            if (!_sessionCache.Contains(sessionId))
            {
                throw new VariableResourceNotFoundException();
            }

            return (Session)_sessionCache[sessionId];
        }

        public Session GetById(Guid sessionId)
        {
            string sessionKey = sessionId.ToString("N");
            if (!_sessionCache.Contains(sessionKey))
            {
                throw new VariableResourceNotFoundException();
            }

            return (Session)_sessionCache[sessionKey];
        }

        public IEnumerable<Session> GetAll()
        {
            return _sessionCache.Select(x => (Session)x.Value);
        }

        public Session Create(Capabilities desiredCapabilities)
        {
            var session = new Session(desiredCapabilities);

            _sessionCache.Add(
                session.SessionKey,
                session,
                new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(5), RemovedCallback = SessionRemoved });

            _logger.Info(String.Format("New Session Created: {0}", session.SessionKey));

            return session;
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