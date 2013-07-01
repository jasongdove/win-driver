using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto.Element
{
    [Route("/session/{sessionId}/element", "POST")]
    [Route("/session/{sessionId}/element/{elementId}/element", "POST")] // TODO: do we need to actually start at the given element id?
    public class LocateElementRequest
    {
        public Guid SessionId { get; set; }
        
        public string Using { get; set; }

        public string Value { get; set; }

        public Guid? ElementId { get; set; }
    }
}