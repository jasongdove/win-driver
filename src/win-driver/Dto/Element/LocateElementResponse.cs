using System;
using System.Collections.Generic;
using WinDriver.Domain;

namespace WinDriver.Dto.Element
{
    public class LocateElementResponse : WebDriverResponse
    {
        public LocateElementResponse(Session session, Guid? elementId)
            : base(session)
        {
            if (elementId.HasValue)
            {
                Value = new Element { Id = elementId.Value.ToString("N") };
            }
            else
            {
                Value = new Dictionary<string, string> { { "message", "no such element" } };
            }
        }
    }
}