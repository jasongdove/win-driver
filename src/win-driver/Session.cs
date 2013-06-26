using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<int> GetWindowHandles()
        {
            return _application.GetWindows().Select(x => x.AutomationElement.Current.NativeWindowHandle);
        }
    }
}