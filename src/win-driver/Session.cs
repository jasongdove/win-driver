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
        public string SessionKey { get { return SessionId.ToString("N"); } }
        public Capabilities Capabilities { get; set; }

        public void Delete()
        {
        }
    }
}