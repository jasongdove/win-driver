using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto.Element
{
    [Route("/session/{sessionId}/element/{elementId}/name", "GET")]
    public class ElementNameRequest
    {
        public Guid SessionId { get; set; }

        public Guid ElementId { get; set; }
    }
}