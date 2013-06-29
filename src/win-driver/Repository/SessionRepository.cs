using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using WinDriver.Domain;
using WinDriver.Exceptions;

namespace WinDriver.Repository
{
    public class SessionRepository : ISessionRepository
    {
        private readonly MemoryCache _cache;

        public SessionRepository()
        {
            _cache = new MemoryCache("Sessions");
        }

        public Session Create(Capabilities capabilities)
        {
            var session = new Session(capabilities);

            _cache.Add(
                session.SessionId.ToString("N"),
                session,
                new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(5) });

            return session;
        }

        public Session GetById(Guid sessionId)
        {
            var session = _cache.Get(sessionId.ToString("N")) as Session;
            
            if (session == null)
            {
                throw new VariableResourceNotFoundException();
            }

            return session;
        }

        public IEnumerable<Session> GetAll()
        {
            return _cache.Select(x => (Session)x.Value);
        }

        public void Delete(Guid sessionId)
        {
            string key = sessionId.ToString("N");

            var session = _cache.Get(key) as Session;
            if (session != null)
            {
                session.Dispose();
                _cache.Remove(key);
            }
        }
    }
}