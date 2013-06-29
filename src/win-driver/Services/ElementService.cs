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

            switch (request.Using.ToUpperInvariant())
            {
                case "NAME":
                    var elementId = session.FindElementByName(request.Value);
                    return new LocateElementResponse(session, elementId);
                default:
                    throw new VariableResourceNotFoundException(); // TODO: should this be method not supported?
            }
        }

        public WebDriverResponse Post(SendKeysRequest request)
        {
            var session = _sessionRepository.GetById(request.SessionId);
            session.SendKeys(request.ElementId, request.Value);
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
    }
}