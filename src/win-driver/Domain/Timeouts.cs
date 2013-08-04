using System;

namespace WinDriver.Domain
{
    public class Timeouts
    {
        public Timeouts()
        {
            Implicit = TimeSpan.Zero;
        }

        public TimeSpan Implicit { get; set; }
    }
}