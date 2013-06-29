using WinDriver.Domain;

namespace WinDriver.Dto
{
    public class TitleResponse : WebDriverResponse
    {
        public TitleResponse(Session session, string title)
            : base(session)
        {
            Value = title;
        }
    }
}