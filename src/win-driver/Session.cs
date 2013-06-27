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
        public string Title { get { return _application.GetWindows().First(x => x.IsCurrentlyActive).Title; } }

        public void Delete()
        {
            _application.Dispose();
        }

        public IEnumerable<int> GetWindowHandles()
        {
            return _application.GetWindows().Select(x => x.AutomationElement.Current.NativeWindowHandle);
        }

        public bool SwitchToWindow(string windowName)
        {
            var window = _application.GetWindow(windowName);
            if (window != null)
            {
                return NativeMethods.SetForegroundWindow(new IntPtr(window.AutomationElement.Current.NativeWindowHandle));
            }

            return false;
        }
    }
}