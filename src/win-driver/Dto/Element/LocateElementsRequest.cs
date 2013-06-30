using System;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto.Element
{
    [Route("/session/{sessionId}/elements", "POST")]
    [Route("/session/{sessionId}/element/{elementId}/elements", "POST")] // TODO: do we need to actually start at the given element id (ignored)?
    public class LocateElementsRequest
    {
        public Guid SessionId { get; set; }

        public string Using { get; set; }

        public string Value { get; set; }

        public Guid? ElementId { get; set; }
    }
}