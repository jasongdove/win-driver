using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto
{
    [Route("/session/{sessionId}/window_handles", "GET")]
    public class WindowHandlesRequest
    {
        public Guid SessionId { get; set; }
    }
}