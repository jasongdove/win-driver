using System.Net.Http;

namespace WinDriver.Repository
{
    public interface IResponseRepository
    {
        HttpResponseMessage Invalid(object request, InvalidRequest reason);
    }
}