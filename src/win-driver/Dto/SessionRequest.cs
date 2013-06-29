using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto
{
    [Route("/session/{sessionId}", "GET,DELETE")]
    public class SessionRequest
    {
        public Guid SessionId { get; set; }
    }
}