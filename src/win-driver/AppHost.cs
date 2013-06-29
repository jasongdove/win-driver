using System;
using System.Configuration;
using Funq;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;

namespace WinDriver
{
    public class AppHost : AppHostHttpListenerBase
    {
        public AppHost() : base("win-driver", typeof (AppHost).Assembly)
        {
        }

        public override void Configure(Container container)
        {
            JsConfig.EmitCamelCaseNames = true;

            SetConfig(new EndpointHostConfig
            {
                GlobalResponseHeaders =
                {
                    { "Cache-Control", "no-cache" }
                },
                DefaultContentType = ContentType.Json
            });
        }

        private static void Main(string[] args)
        {
            var appHost = new AppHost();
            appHost.Init();

            string endpoint = String.Format(
                "http://{0}:{1}/",
                ConfigurationManager.AppSettings["ip"],
                ConfigurationManager.AppSettings["port"]);

            appHost.Start(endpoint);

            Console.ReadLine();
        }
    }
}
