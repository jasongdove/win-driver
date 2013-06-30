using System;
using System.Globalization;
using System.Linq;
using ServiceStack.ServiceInterface;
using WinDriver.Dto;
using WinDriver.Exceptions;
using WinDriver.Repository;

namespace WinDriver.Services
{
    public class WindowService : Service
    {
        private readonly ISessionRepository _sessionRepository;

        public WindowService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public WindowHandlesResponse Get(WindowHandlesRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            return new WindowHandlesResponse(session, session.GetWindowHandles().ToArray());
        }

        public TitleResponse Get(TitleRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            return new TitleResponse(session, session.Title);
        }

        public WebDriverResponse Post(SwitchToWindowRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);

            if (String.IsNullOrEmpty(request.Name))
            {
                throw new MissingCommandParameterException();
            }

            session.SwitchToWindow(request.Name);

            // TODO: return failure if we aren't able to switch
            return new WebDriverResponse(session) { Status = 0 };
        }

        public WebDriverResponse Get(WindowHandleRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            return new WebDriverResponse(session)
            {
                Status = 0,
                Value = session.GetWindowHandle().ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}