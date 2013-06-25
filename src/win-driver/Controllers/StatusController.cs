using System;
using System.Reflection;

namespace WinDriver
{
    public class StatusController : WebDriverApiController
    {
        public object Get()
        {
            var status = new
            {
                build = new
                {
                    version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
                },
                os = new
                {
                    name = Environment.OSVersion.Platform.ToString(),
                    version = Environment.OSVersion.Version.ToString(),
                    arch = Environment.Is64BitProcess ? "64bit" : "32bit"
                }
            };

            return Success(status);
        }
    }
}