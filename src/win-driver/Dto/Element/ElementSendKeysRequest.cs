using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto.Element
{
    [Route("/session/{sessionId}/element/{elementId}/value", "POST")]
    public class ElementSendKeysRequest
    {
        public Guid SessionId { get; set; }

        public Guid ElementId { get; set; }

        public char[] Value { get; set; }
    }
}