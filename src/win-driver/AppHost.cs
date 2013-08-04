using System;
using System.Configuration;
using Funq;
using ServiceStack.Common.Web;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using WinDriver.Exceptions;
using WinDriver.Repository;
using WinDriver.Services.Automation;

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
            LogManager.LogFactory = new ConsoleLogFactory();

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

            container.Register<ILog>(x => LogManager.LogFactory.GetLogger("win-driver"));
            container.Register<IElementRepository>(new ElementRepository());
            container.Register<ISessionRepository>(new SessionRepository(container.Resolve<ILog>(), container.Resolve<IElementRepository>()));
            container.Register<IAutomationService>(new WhiteAutomationService(container.Resolve<IElementRepository>()));
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
