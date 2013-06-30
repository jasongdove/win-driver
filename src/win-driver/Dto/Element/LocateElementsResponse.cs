using System;
using System.Collections.Generic;
using System.Linq;
using WinDriver.Domain;

namespace WinDriver.Dto.Element
{
    public class LocateElementsResponse : WebDriverResponse
    {
        public LocateElementsResponse(Session session, IEnumerable<Guid> elementIds)
            : base(session)
        {
            Value = elementIds.Select(x => new Element { Id = x.ToString("N") }).ToArray();
        }
    }
}