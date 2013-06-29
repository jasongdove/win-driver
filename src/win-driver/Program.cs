using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.SelfHost;
using Castle.Core.Internal;
using Newtonsoft.Json.Serialization;
using Ninject;
using Ninject.Web.Common.SelfHost;
using WinDriver.Repository;


namespace WinDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            var ip = ConfigurationManager.AppSettings["ip"];
            var port = ConfigurationManager.AppSettings["port"];

            var config = new HttpSelfHostConfiguration(String.Format("http://{0}:{1}", ip, port));

            config.Routes.MapHttpRoute(
                "Shutdown",
                "wd/hub/shutdown",
                new { controller = "Shutdown", action = "Default" });

            config.Routes.MapHttpRoute(
                "Sessions",
                "wd/hub/sessions",
                new { controller = "Session", action = "GetAll" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                "SessionElementPost",
                "wd/hub/session/{sessionId}/element",
                new { controller = "Element", action = "Find" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute(
                "DefaultApiWithId",
                "wd/hub/{controller}/{id}/{action}",
                new { id = RouteParameter.Optional, action = "DefaultAction" });

            config.Routes.MapHttpRoute(
                "SessionElementAction",
                "wd/hub/session/{sessionId}/element/{id}/{action}",
                new { controller = "Element" });

            config.Routes.MapHttpRoute(
                "DefaultApiGet",
                "wd/hub/{controller}",
                new { action = "Get" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute(
                "DefaultApiPost",
                "wd/hub/{controller}",
                new { action = "Post" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(x => x.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var kernel = CreateKernel();
            using (var server = new NinjectSelfHostBootstrapper(() => kernel, config))
            {
                server.Start();
                Console.ReadLine();
                kernel.Get<ISessionRepository>().GetAll().ForEach(x => x.Delete());
            }
        }

        private static StandardKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            return kernel;
        }
    }
}
