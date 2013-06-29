using System.Collections.Generic;

namespace WinDriver.Domain
{
    public class Capabilities
    {
        public Capabilities(Dictionary<string, object> capabilities)
        {
            if (capabilities.ContainsKey("app"))
            {
                App = capabilities["app"] as string;
            }
        }

        public string DriverName
        {
            get { return "win-driver"; }
        }

        public string DriverVersion
        {
            get { return typeof (AppHost).Assembly.GetName().Version.ToString(); }
        }

        public string App { get; private set; }
    }
}