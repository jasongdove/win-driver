using System;
using System.Linq;
using System.Net;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using ServiceStack.Text;
using WinDriver.Domain;
using WinDriver.Dto;
using WinDriver.Exceptions;
using WinDriver.Repository;
using WinDriver.Services.Automation;

namespace WinDriver.Services
{
    public class SessionService : Service
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IAutomationService _automationService;

        public SessionService(ISessionRepository sessionRepository, IAutomationService automationService)
        {
            _sessionRepository = sessionRepository;
            _automationService = automationService;
        }

        public object Post(NewSessionRequest request)
        {
            if (request.RequiredCapabilities != null)
            {
                // TODO: verify that required capabilities can be supported, otherwise return session_not_created
            }

            if (request.DesiredCapabilities != null)
            {
                // TODO: verify that app exists, otherwise return session_not_created

                // TODO: support more desired capabilities, rather than just ignoring them
                var capabilities = new Capabilities(request.DesiredCapabilities);
                var session = _sessionRepository.Create(capabilities);

                return new HttpResult
                {
                    StatusCode = HttpStatusCode.RedirectMethod,
                    Headers =
                    {
                        { HttpHeaders.Location, RequestContext.AbsoluteUri.WithTrailingSlash() + session.SessionId.ToString("N") }
                    }
                };
            }

            throw new MissingCommandParameterException();
        }

        public SessionResponse Get(SessionRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            return new SessionResponse(session);
        }

        public WebDriverResponse Delete(SessionRequest request)
        {
            _sessionRepository.Delete(request.SessionId);
            return new WebDriverResponse();
        }

        public SessionsResponse Get(SessionsRequest request)
        {
            var response = new SessionsResponse();
            response.AddRange(_sessionRepository.GetAll().Select(x => new SessionResponse(x)));
            return response;
        }

        public WebDriverResponse Post(MoveToRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            _automationService.MoveTo(session, request.Element, request.XOffset, request.YOffset);
            return new WebDriverResponse(session) { Status = StatusCode.Success };
        }

        public WebDriverResponse Post(DoubleClickRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            _automationService.DoubleClick(session);
            return new WebDriverResponse(session) { Status = StatusCode.Success };
        }

        public WebDriverResponse Post(ImplicitWaitRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            session.Timeouts.Implicit = TimeSpan.FromMilliseconds(request.Ms);
            return new WebDriverResponse(session) { Status = StatusCode.Success };
        }

        public WebDriverResponse Post(SessionSendKeysRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            _automationService.SendKeys(session, request.Value);
            return new WebDriverResponse(session) { Status = StatusCode.Success };
        }
    }
}