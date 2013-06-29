using WinDriver.Domain;

namespace WinDriver.Dto
{
    public class SessionResponse : WebDriverResponse
    {
        public SessionResponse(Session session)
        {
            SessionId = session.SessionId.ToString("N");
            Status = 0;
            Value = session.Capabilities.Dto;
        }
    }
}