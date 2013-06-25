using System;
using System.Net.Http;
using System.Text;
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

            Task.Factory.StartNew(() => Task.Delay(1000).Wait()).ContinueWith(t => Environment.Exit(0));

            return message;
        }
    }
}