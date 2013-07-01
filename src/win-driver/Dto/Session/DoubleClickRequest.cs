using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto
{
    [Route("/session/{sessionId}/doubleclick", "POST")]
    public class DoubleClickRequest
    {
        public Guid SessionId { get; set; }
    }
}