using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto
{
    [Route("/session/{sessionId}/window_handle", "GET")]
    public class WindowHandleRequest
    {
        public Guid SessionId { get; set; }
    }
}