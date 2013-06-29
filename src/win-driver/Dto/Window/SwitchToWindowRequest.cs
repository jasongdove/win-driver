using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto
{
    [Route("/session/{sessionId}/window", "POST")]
    public class SwitchToWindowRequest
    {
        public Guid SessionId { get; set; }

        public string Name { get; set; }
    }
}