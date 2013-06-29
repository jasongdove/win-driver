using Ninject.Modules;
using WinDriver.Repository;

namespace WinDriver
{
    public class Module : NinjectModule
    {
        public override void Load()
        {
            Bind<ISessionRepository>().To<SessionRepository>().InSingletonScope();
            Bind<IResponseRepository>().To<ResponseRepository>();
        }
    }
}