using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto.Element
{
    [Route("/session/{sessionId}/element/{elementId}/click", "POST")]
    public class ClickElementRequest
    {
        public Guid SessionId { get; set; }

        public Guid ElementId { get; set; }
    }
}