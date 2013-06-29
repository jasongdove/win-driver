using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using White.Core;
using White.Core.UIItems.Finders;

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
                Thread.Sleep(3000);
                _application.WaitWhileBusy();
            }
        }

        public Guid SessionId { get; private set; }
        public string SessionKey { get { return SessionId.ToString("N"); } }
        public Capabilities Capabilities { get; set; }
        
        public string Title
        {
            get
            {
                var window = _application.GetWindows().FirstOrDefault(x => x.IsCurrentlyActive);
                return window != null ? window.Title : _application.Process.MainWindowTitle;
            }
        }

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

        public string FindElementByName(string name)
        {
            var window = _application.GetWindow(Title);

            var byText = window.GetElement(SearchCriteria.ByText(name));
            if (byText != null)
            {
                return Guid.NewGuid().ToString("N"); // TODO: return element cache
            }

            var byAutomationId = window.GetElement(SearchCriteria.ByAutomationId(name));
            if (byAutomationId != null)
            {
                return Guid.NewGuid().ToString("N"); // TODO: return element cache
            }

            return null;
        }
    }
}