using System;
using ServiceStack.ServiceInterface;
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

            var elementId = session.FindElement(request.Using, request.Value);
            return new LocateElementResponse(session, elementId);
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

        public WebDriverResponse Post(SendKeysRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            session.SendKeys(request.ElementId, request.Value);
            return new WebDriverResponse(session) { Status = 0 };
        }

        public WebDriverResponse Post(ClickElementRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            session.Click(request.ElementId);
            return new WebDriverResponse(session) { Status = 0 };
        }

        public WebDriverResponse Post(ClearElementRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            session.Clear(request.ElementId);
            return new WebDriverResponse(session) { Status = 0 };
        }

        public WebDriverResponse Get(ElementNameRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            var name = session.GetElementName(request.ElementId);
            return new WebDriverResponse(session) { Status = 0, Value = name.ToLowerInvariant() };
        }

        public WebDriverResponse Get(ElementAttributeRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            var value = session.GetElementAttribute(request.ElementId, request.Name);
            return new WebDriverResponse(session) { Status = 0, Value = value };
        }

        public WebDriverResponse Get(ElementTextRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            var value = session.GetElementText(request.ElementId);
            return new WebDriverResponse(session) { Status = 0, Value = value };
        }
    }
}