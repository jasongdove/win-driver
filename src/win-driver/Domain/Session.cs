using System;
using White.Core;

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

        public void Dispose()
        {
            if (_application != null)
            {
                _application.Dispose();
            }
        }
    }
}