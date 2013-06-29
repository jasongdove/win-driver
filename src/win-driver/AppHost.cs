using System;
using System.Configuration;
using Funq;
using ServiceStack.Common.Web;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using WinDriver.Exceptions;
using WinDriver.Repository;

namespace WinDriver
{
    public class AppHost : AppHostHttpListenerBase
    {
        public AppHost()
            : base("win-driver", typeof (AppHost).Assembly)
        {
        }

        public override void Configure(Container container)
        {
            JsConfig.EmitCamelCaseNames = true;
            JsConfig.IncludeNullValues = true;
            JsConfig.ExcludeTypeInfo = true;

            SetConfig(new EndpointHostConfig
            {
                GlobalResponseHeaders =
                {
                    { "Cache-Control", "no-cache" }
                },
                DefaultContentType = ContentType.Json,
                MapExceptionToStatusCode =
                {
                    { typeof(VariableResourceNotFoundException), 404 }, // not found
                    { typeof(MissingCommandParameterException), 400 }, // bad request
                }
            });

            container.Register<ISessionRepository>(new SessionRepository());
        }

        private static void Main(string[] args)
        {
            var appHost = new AppHost();
            appHost.Init();

            string endpoint = String.Format(
                "http://{0}:{1}/wd/hub/",
                ConfigurationManager.AppSettings["ip"],
                ConfigurationManager.AppSettings["port"]);

            appHost.Start(endpoint);

            Console.ReadLine();

            var repository = appHost.Container.Resolve<ISessionRepository>();
            repository.Dispose();
        }
    }
}
