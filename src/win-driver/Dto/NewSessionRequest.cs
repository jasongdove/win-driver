using System.Collections.Generic;
using ServiceStack.ServiceHost;

namespace WinDriver.Dto
{
    [Route("/session", "POST")]
    public class NewSessionRequest
    {
        public Dictionary<string, object> RequiredCapabilities { get; set; }
        
        public Dictionary<string, object> DesiredCapabilities { get; set; }
    }
}