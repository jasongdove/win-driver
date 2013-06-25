using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Ninject.Extensions.Logging;

namespace WinDriver.Controllers
{
    public class ShutdownController : WebDriverApiController
    {
        private readonly ILogger _logger;

        public ShutdownController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpGet, HttpPost, HttpPut, HttpHead]
        public HttpResponseMessage Default()
        {
            _logger.Info("About to shutdown");

            var message = new HttpResponseMessage
            {
                Content = new StringContent("<html><body>Closing...</body></html>", Encoding.UTF8, "text/html")
            };

            // TODO: find a better way to do this after the response has been sent
            Task.Factory.StartNew(() => { Thread.Sleep(2000); Environment.Exit(0); });

            return message;
        }
    }
}