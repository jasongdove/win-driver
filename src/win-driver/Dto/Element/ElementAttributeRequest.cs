using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto.Element
{
    [Route("/session/{sessionId}/element/{elementId}/attribute/{name}", "GET")]
    public class ElementAttributeRequest
    {
        public Guid SessionId { get; set; }

        public Guid ElementId { get; set; }

        public string Name { get; set; }
    }
}