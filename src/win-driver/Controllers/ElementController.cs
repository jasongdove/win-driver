using System;
using Newtonsoft.Json.Linq;
using WinDriver.Exceptions;
using WinDriver.Repository;

namespace WinDriver.Controllers
{
    public class ElementController : WebDriverApiController
    {
        private readonly ISessionRepository _sessionRepository;

        public ElementController(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public object Find(string sessionId, JObject parameters)
        {
            var session = _sessionRepository.GetById(sessionId);

            if (parameters["using"] == null || parameters["value"] == null)
            {
                throw new MissingCommandParameterException();
            }

            var locatorStrategy = parameters["using"].Value<string>();
            var value = parameters["value"].Value<string>();

            switch (locatorStrategy.ToUpperInvariant())
            {
                case "NAME":
                    var element = session.FindElementByName(value);
                    if (!String.IsNullOrEmpty(element))
                    {
                        return Success(
                            session.SessionId,
                            new JObject { { "ELEMENT", JToken.FromObject(element) } });
                    }
                    break;
                default:
                    // TODO: return NoSuchElement -- or should this be method not supported?
                    break;
            }

            // TODO: return NoSuchElement
            return Success(session.SessionId, null);
        }
    }
}