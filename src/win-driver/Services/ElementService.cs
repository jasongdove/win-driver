using System;
using ServiceStack.ServiceInterface;
using WinDriver.Domain;
using WinDriver.Dto;
using WinDriver.Dto.Element;
using WinDriver.Exceptions;
using WinDriver.Repository;
using WinDriver.Services.Automation;

namespace WinDriver.Services
{
    public class ElementService : Service
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IAutomationService _automationService;

        public ElementService(ISessionRepository sessionRepository, IAutomationService automationService)
        {
            _sessionRepository = sessionRepository;
            _automationService = automationService;
        }

        public LocateElementResponse Post(LocateElementRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);

            if (String.IsNullOrEmpty(request.Using) || String.IsNullOrEmpty(request.Value))
            {
                throw new MissingCommandParameterException();
            }

            var elementId = _automationService.FindElement(session, request.Using, request.Value, request.ElementId);
            var status = elementId.HasValue ? StatusCode.Success : StatusCode.NoSuchElement;
            return new LocateElementResponse(session, elementId) { Status = status };
        }

        public LocateElementsResponse Post(LocateElementsRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);

            if (String.IsNullOrEmpty(request.Using) || String.IsNullOrEmpty(request.Value))
            {
                throw new MissingCommandParameterException();
            }

            var elementIds = _automationService.FindElements(session, request.Using, request.Value, request.ElementId);
            return new LocateElementsResponse(session, elementIds);
        }

        public WebDriverResponse Post(ElementSendKeysRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            _automationService.SendKeys(session, request.ElementId, request.Value);
            return new WebDriverResponse(session) { Status = StatusCode.Success };
        }

        public WebDriverResponse Post(ClickElementRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            _automationService.Click(session, request.ElementId);
            return new WebDriverResponse(session) { Status = StatusCode.Success };
        }

        public WebDriverResponse Post(ClearElementRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            _automationService.Clear(session, request.ElementId);
            return new WebDriverResponse(session) { Status = StatusCode.Success };
        }

        public WebDriverResponse Get(ElementNameRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            var name = _automationService.GetElementName(session, request.ElementId);
            return new WebDriverResponse(session) { Status = StatusCode.Success, Value = name.ToLowerInvariant() };
        }

        public WebDriverResponse Get(ElementAttributeRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            var value = _automationService.GetElementAttribute(session, request.ElementId, request.Name);
            return new WebDriverResponse(session) { Status = StatusCode.Success, Value = value };
        }

        public WebDriverResponse Get(ElementTextRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            var value = _automationService.GetElementText(session, request.ElementId);
            return new WebDriverResponse(session) { Status = StatusCode.Success, Value = value };
        }
    }
}