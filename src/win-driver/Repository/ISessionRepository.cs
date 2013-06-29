using System;
using System.Collections.Generic;
using WinDriver.Domain;

namespace WinDriver.Repository
{
    public interface ISessionRepository
    {
        Session Create(Capabilities capabilities);

        Session GetById(Guid sessionId);

        IEnumerable<Session> GetAll();

        void Delete(Guid sessionId);
    }
}