using System;
using System.Threading;
using ServiceStack.Logging;
using White.Core;
using White.Core.UIItems.WindowItems;
using WinDriver.Repository;

namespace WinDriver.Domain
{
    public sealed class Session : IDisposable
    {
        private readonly ILog _log;
        private readonly IElementRepository _elementRepository;
        private readonly Capabilities _capabilities;
        private readonly Guid _sessionId;
        private readonly Application _application;
        private readonly Timeouts _timeouts;
        private Window _window;

        public Session(ILog log, IElementRepository elementRepository, Capabilities capabilities)
        {
            _log = log;
            _elementRepository = elementRepository;
            _capabilities = capabilities;
            _sessionId = Guid.NewGuid();

            // TODO: inject this somehow

            if (Capabilities.App != null)
            {
                _application = Application.Launch(Capabilities.App);

                while (_application.GetWindows().Count == 0)
                {
                    Thread.Sleep(500);
                }

                _application.WaitWhileBusy();
            }

            _timeouts = new Timeouts();
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
            get { return _application.Process.MainWindowTitle; }
        }

        public Timeouts Timeouts
        {
            get { return _timeouts; }
        }

        public Application Application
        {
            get { return _application; }
        }

        public Window Window
        {
            get { return _window; }
            set { _window = value; }
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