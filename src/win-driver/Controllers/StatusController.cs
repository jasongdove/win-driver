using System;
using System.Reflection;

namespace WinDriver.Controllers
{
    public class StatusController : WebDriverApiController
    {
        public object Get()
        {
            string platform;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                    platform = "mac";
                    break;
                case PlatformID.Unix:
                    platform = "unix";
                    break;
                default:
                    platform = "windows";
                    break;
            }

            var status = new
            {
                build = new
                {
                    version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                },
                os = new
                {
                    name = platform,
                    version = Environment.OSVersion.Version.ToString(),
                    arch = Environment.Is64BitProcess ? "64bit" : "32bit"
                }
            };

            return Success(status);
        }
    }
}