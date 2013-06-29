using System;

namespace WinDriver.Domain
{
    public sealed class Session : IDisposable
    {
        private readonly Capabilities _capabilities;
        private readonly Guid _sessionId;

        public Session(Capabilities capabilities)
        {
            _capabilities = capabilities;
            _sessionId = Guid.NewGuid();
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
            // TODO: clean up
        }
    }
}