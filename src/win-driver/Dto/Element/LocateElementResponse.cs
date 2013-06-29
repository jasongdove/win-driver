using System;
using WinDriver.Domain;

namespace WinDriver.Dto.Element
{
    public class LocateElementResponse : WebDriverResponse
    {
        public LocateElementResponse(Session session, Guid elementId)
            : base(session)
        {
            Value = new Element { Id = elementId.ToString("N") };
        }
    }
}