using System;
using White.Core;

namespace WinDriver
{
    public class Session
    {
        private readonly Application _application;

        public Session()
            : this(Capabilities.GetDefaultCapabilities())
        {
        }

        public Session(Capabilities capabilities)
        {
            SessionId = Guid.NewGuid();
            Capabilities = capabilities;

            if (Capabilities.App != null)
            {
                _application = Application.Launch(Capabilities.App);
            }
        }

        public Guid SessionId { get; private set; }
        public string SessionKey { get { return SessionId.ToString("N"); } }
        public Capabilities Capabilities { get; set; }

        public void Delete()
        {
            _application.Dispose();
        }
    }
}