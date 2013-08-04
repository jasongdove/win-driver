using System;
using System.Collections.Generic;
using WinDriver.Domain;

namespace WinDriver.Services.Automation
{
    public interface IAutomationService
    {
        void Click(Session session, Guid elementId);

        void DoubleClick(Session session);

        void MoveTo(Session session, Guid? elementId, int? xOffset, int? yOffset);

        string GetElementText(Session session, Guid elementId);

        string GetElementAttribute(Session session, Guid elementId, string attribute);

        int GetWindowHandle(Session session);

        IEnumerable<int> GetWindowHandles(Session session);

        bool SwitchToWindow(Session session, string title);

        string GetElementName(Session session, Guid elementId);

        void Clear(Session session, Guid elementId);

        Guid? FindElement(Session session, string locator, string value, Guid? elementId);

        IEnumerable<Guid> FindElements(Session session, string locator, string value, Guid? elementId);

        void SendKeys(Session session, Guid elementId, char[] keys);

        void SendKeys(Session session, string[] keys);
    }
}