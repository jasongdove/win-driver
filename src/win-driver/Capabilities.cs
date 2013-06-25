using System.Reflection;

namespace WinDriver
{
    public class Capabilities
    {
        public string DriverName { get; set; }
        public string DriverVersion { get; set; }

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