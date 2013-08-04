using System;

using ServiceStack.ServiceHost;

namespace WinDriver.Dto
{
    [Route("/session/{sessionId}/timeouts/implicit_wait", "POST")]
    public class ImplicitWaitRequest
    {
        public Guid SessionId { get; set; }

        public double Ms { get; set; }
    }
}