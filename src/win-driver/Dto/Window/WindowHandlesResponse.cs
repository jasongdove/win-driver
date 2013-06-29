using WinDriver.Domain;

namespace WinDriver.Dto
{
    public class WindowHandlesResponse : WebDriverResponse
    {
        public WindowHandlesResponse(Session session, int[] handles)
            : base(session)
        {
            Value = handles;
        }
    }
}