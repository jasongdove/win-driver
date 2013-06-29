using WinDriver.Domain;

namespace WinDriver.Dto
{
    public class WebDriverResponse
    {
        public WebDriverResponse()
        {
        }

        public WebDriverResponse(Session session)
        {
            SessionId = session.SessionId.ToString("N");
        }

        public string SessionId { get; set; }

        public int Status { get; set; }

        public object Value { get; set; }
    }
}