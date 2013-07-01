using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto
{
    [Route("/session/{sessionId}/moveto", "POST")]
    public class MoveToRequest
    {
        public Guid SessionId { get; set; }
        
        public Guid? Element { get; set; }

        public int? XOffset { get; set; }
        
        public int? YOffset { get; set; }
    }
}