using System.Web.Http.Filters;
using Ninject;
using WinDriver.Exceptions;
using WinDriver.Repository;

namespace WinDriver.Filters
{
    public class SessionExceptionFilterAttribute : ExceptionFilterAttribute
    {
        [Inject]
        private IResponseRepository ResponseRepository { get; set; }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is MissingCommandParameterException)
            {
                actionExecutedContext.Response = ResponseRepository.Invalid(
                    null,
                    InvalidRequest.MissingCommandParameter);
            }
            else if (actionExecutedContext.Exception is VariableResourceNotFoundException)
            {
                actionExecutedContext.Response = ResponseRepository.Invalid(
                    null,
                    InvalidRequest.VariableResourceNotFound);
            }
        }
    }
}