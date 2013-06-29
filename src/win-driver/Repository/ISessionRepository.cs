using System;
using System.Collections.Generic;

namespace WinDriver.Repository
{
    public interface ISessionRepository
    {
        Session GetById(string sessionId);
        Session GetById(Guid sessionId);
        IEnumerable<Session> GetAll();
        Session Create(Capabilities desiredCapabilities);
    }
}