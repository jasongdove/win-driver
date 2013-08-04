using System;
using ServiceStack.ServiceInterface;
using WinDriver.Domain;
using WinDriver.Dto;
using WinDriver.Dto.Element;
using WinDriver.Exceptions;
using WinDriver.Repository;

namespace WinDriver.Services
{
    public class ElementService : Service
    {
        private readonly ISessionRepository _sessionRepository;

        public ElementService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public LocateElementResponse Post(LocateElementRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);

            if (String.IsNullOrEmpty(request.Using) || String.IsNullOrEmpty(request.Value))
            {
                throw new MissingCommandParameterException();
            }

            var elementId = session.FindElement(request.Using, request.Value, request.ElementId);
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

            var elementIds = session.FindElements(request.Using, request.Value, request.ElementId);
            return new LocateElementsResponse(session, elementIds);
        }

        public WebDriverResponse Post(ElementSendKeysRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            session.SendKeys(request.ElementId, request.Value);
            return new WebDriverResponse(session) { Status = StatusCode.Success };
        }

        public WebDriverResponse Post(ClickElementRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            session.Click(request.ElementId);
            return new WebDriverResponse(session) { Status = StatusCode.Success };
        }

        public WebDriverResponse Post(ClearElementRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            session.Clear(request.ElementId);
            return new WebDriverResponse(session) { Status = StatusCode.Success };
        }

        public WebDriverResponse Get(ElementNameRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            var name = session.GetElementName(request.ElementId);
            return new WebDriverResponse(session) { Status = StatusCode.Success, Value = name.ToLowerInvariant() };
        }

        public WebDriverResponse Get(ElementAttributeRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            var value = session.GetElementAttribute(request.ElementId, request.Name);
            return new WebDriverResponse(session) { Status = StatusCode.Success, Value = value };
        }

        public WebDriverResponse Get(ElementTextRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            var value = session.GetElementText(request.ElementId);
            return new WebDriverResponse(session) { Status = StatusCode.Success, Value = value };
        }
    }
}