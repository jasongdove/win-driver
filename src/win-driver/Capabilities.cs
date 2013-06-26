using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace WinDriver
{
    public class Capabilities
    {
        public Capabilities(IDictionary<string, JToken> capabilities)
        {
            if (capabilities.ContainsKey("app"))
            {
                App = capabilities["app"].Value<string>();
            }
        }

        private Capabilities()
        {
        }

        public string DriverName { get; set; }
        public string DriverVersion { get; set; }
        public string App { get; set; }

        public static Capabilities GetDefaultCapabilities()
        {
            return new Capabilities
            {
                DriverName = "win-driver",
                DriverVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
            };
        }
    }
}