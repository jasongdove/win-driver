using System;
using ServiceStack.ServiceInterface;
using WinDriver.Dto;
using OperatingSystem = WinDriver.Dto.OperatingSystem;

namespace WinDriver.Services
{
    public class StatusService : Service
    {
        public StatusResponse Get(StatusRequest request)
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

            return new StatusResponse
            {
                Build = new Build
                {
                    Version = typeof(AppHost).Assembly.GetName().Version.ToString()
                },
                OS = new OperatingSystem
                {
                    Name = platform,
                    Version = Environment.OSVersion.Version.ToString(),
                    Arch = Environment.Is64BitProcess ? "64bit" : "32bit"
                }
            };
        }
    }
}