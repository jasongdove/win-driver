using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto.Element
{
    [Route("/session/{sessionId}/element/{elementId}/text", "GET")]
    public class ElementTextRequest
    {
        public Guid SessionId { get; set; }

        public Guid ElementId { get; set; }
    }
}