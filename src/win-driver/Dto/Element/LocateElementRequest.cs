using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto.Element
{
    [Route("/session/{sessionId}/element", "POST")]
    public class LocateElementRequest
    {
        public Guid SessionId { get; set; }
        
        public string Using { get; set; }

        public string Value { get; set; }
    }
}