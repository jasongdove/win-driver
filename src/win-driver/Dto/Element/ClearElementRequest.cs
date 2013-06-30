using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto.Element
{
    [Route("/session/{sessionId}/element/{elementId}/clear", "POST")]
    public class ClearElementRequest
    {
        public Guid SessionId { get; set; }
        
        public Guid ElementId { get; set; }
    }
}