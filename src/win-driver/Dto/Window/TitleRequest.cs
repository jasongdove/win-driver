using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto
{
    [Route("/session/{sessionId}/title", "GET")]
    public class TitleRequest
    {
        public Guid SessionId { get; set; }         
    }
}