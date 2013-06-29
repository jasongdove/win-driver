using System;
using ServiceStack.ServiceInterface;
using WinDriver.Dto;

namespace WinDriver.Services
{
    public class ShutdownService : Service
    {
        public void Any(ShutdownRequest request)
        {
            Response.ContentType = "text/html";
            Response.Write("<html><body>Closing...</body></html>");
            Response.Close();

            Environment.Exit(0);
        }
    }
}