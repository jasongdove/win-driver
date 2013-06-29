using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using White.Core;
using White.Core.Factory;
using White.Core.UIItems;

namespace WinDriver.Domain
{
    public sealed class Session : IDisposable
    {
        private readonly Capabilities _capabilities;
        private readonly Guid _sessionId;

        private readonly Application _application;

        public Session(Capabilities capabilities)
        {
            _capabilities = capabilities;
            _sessionId = Guid.NewGuid();

            if (_capabilities.App != null)
            {
                _application = Application.Launch(Capabilities.App);

                while (_application.GetWindows().Count == 0)
                {
                    Thread.Sleep(500);
                }

                _application.WaitWhileBusy();
            }
        }

        public Capabilities Capabilities
        {
            get { return _capabilities; }
        }

        public Guid SessionId
        {
            get { return _sessionId; }
        }

        public string Title
        {
            get
            {
                var window = _application.GetWindows().FirstOrDefault(x => x.IsCurrentlyActive);
                return window != null ? window.Title : _application.Process.MainWindowTitle;
            }
        }

        public IEnumerable<int> GetWindowHandles()
        {
            return _application.GetWindows().Select(x => x.AutomationElement.Current.NativeWindowHandle);
        }

        public bool SwitchToWindow(string title)
        {
            try
            {
                var window = _application.GetWindow(title, InitializeOption.NoCache);
                return NativeMethods.SetForegroundWindow(new IntPtr(window.AutomationElement.Current.NativeWindowHandle));
            }
            catch (UIActionException)
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (_application != null)
            {
                _application.Dispose();
            }
        }
    }
}