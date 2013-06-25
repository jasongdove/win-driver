using System;

namespace WinDriver
{
    public class Session
    {
        public Session()
        {
            SessionId = Guid.NewGuid();
            Capabilities = Capabilities.GetDefaultCapabilities();
        }

        public Guid SessionId { get; private set; }
        public Capabilities Capabilities { get; set; }
    }
}