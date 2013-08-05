using System;
using System.Diagnostics;
using System.Threading;
using TestStack.White;

namespace WinDriver.Domain
{
    public sealed class Session : IDisposable
    {
        private readonly Capabilities _capabilities;
        private readonly Guid _sessionId;
        private readonly Process _process;
        private readonly Timeouts _timeouts;

        public Session(Capabilities capabilities)
        {
            _capabilities = capabilities;
            _sessionId = Guid.NewGuid();

            // TODO: inject this somehow

            if (Capabilities.App != null)
            {
                // TODO: Not sure why this works, but Process.Start will throw COMExceptions once automation is used...
                var app = Application.Launch(Capabilities.App);
                
                while (app.GetWindows().Count == 0)
                {
                    Thread.Sleep(500);
                }

                app.WaitWhileBusy();

                _process = app.Process;
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

        public Timeouts Timeouts
        {
            get { return _timeouts; }
        }

        public Process Process
        {
            get { return _process; }
        }

        public void Dispose()
        {
            if (_process != null)
            {
                if (_process.HasExited)
                {
                    _process.Dispose();
                    return;
                }

                _process.CloseMainWindow();
                _process.WaitForExit(5000);

                if (!_process.HasExited)
                {
                    _process.Kill();
                }

                _process.Dispose();
            }
        }
    }
}