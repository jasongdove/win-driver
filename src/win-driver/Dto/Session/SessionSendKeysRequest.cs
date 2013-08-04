using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto
{
    [Route("/session/{sessionId}/keys", "POST")]
    public class SessionSendKeysRequest
    {
        public Guid SessionId { get; set; }

        public string[] Value { get; set; }
    }
}