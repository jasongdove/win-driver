using System;
using System.Globalization;
using System.Linq;
using ServiceStack.ServiceInterface;
using WinDriver.Domain;
using WinDriver.Dto;
using WinDriver.Exceptions;
using WinDriver.Repository;
using WinDriver.Services.Automation;

namespace WinDriver.Services
{
    public class WindowService : Service
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IAutomationService _automationService;

        public WindowService(ISessionRepository sessionRepository, IAutomationService automationService)
        {
            _sessionRepository = sessionRepository;
            _automationService = automationService;
        }

        public WindowHandlesResponse Get(WindowHandlesRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            var handles = _automationService.GetWindowHandles(session).ToArray();
            return new WindowHandlesResponse(session, handles);
        }

        public TitleResponse Get(TitleRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            return new TitleResponse(session, _automationService.GetTitle(session));
        }

        public WebDriverResponse Post(SwitchToWindowRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);

            if (String.IsNullOrEmpty(request.Name))
            {
                throw new MissingCommandParameterException();
            }

            _automationService.SwitchToWindow(session, request.Name);

            // TODO: return failure if we aren't able to switch
            return new WebDriverResponse(session) { Status = StatusCode.Success };
        }

        public WebDriverResponse Get(WindowHandleRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            return new WebDriverResponse(session)
            {
                Status = StatusCode.Success,
                Value = _automationService.GetWindowHandle(session).ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}