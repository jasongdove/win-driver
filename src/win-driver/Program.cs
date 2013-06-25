using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.SelfHost;
using Ninject;
using Ninject.Web.Common.SelfHost;


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
                "DefaultApiGet",
                "wd/hub/{controller}",
                new { action = "Get" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(x => x.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            using (var server = new NinjectSelfHostBootstrapper(CreateKernel, config))
            {
                server.Start();
                Console.ReadLine();
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
