using System;
using System.Collections.Generic;
using interop.UIAutomationCore;
using WinDriver.Domain;
using WinDriver.Exceptions;
using WinDriver.Repository;

namespace WinDriver.Services.Automation
{
    public class UIAutomationService : IAutomationService
    {
        private readonly IUIAutomation _automation;
        private readonly IElementRepository _elementRepository;

        public UIAutomationService(IElementRepository elementRepository)
        {
            _automation = new CUIAutomation();
            _elementRepository = elementRepository;
        }

        public void Click(Session session, Guid elementId)
        {
            var element = GetUIAutomationElement(elementId);
            var invokePattern = (IUIAutomationInvokePattern)element.GetCurrentPattern(UIA_PatternIds.UIA_InvokePatternId);
            if (invokePattern != null)
            {
                invokePattern.Invoke();
            }
            else
            {
                var selectionItemPattern = (IUIAutomationSelectionItemPattern)element.GetCurrentPattern(UIA_PatternIds.UIA_SelectionItemPatternId);
                selectionItemPattern.Select();
            }
        }

        public void DoubleClick(Session session)
        {
            throw new NotImplementedException();
        }

        public void MoveTo(Session session, Guid? elementId, int? xOffset, int? yOffset)
        {
            throw new NotImplementedException();
        }

        public string GetElementText(Session session, Guid elementId)
        {
            throw new NotImplementedException();
        }

        public string GetElementAttribute(Session session, Guid elementId, string attribute)
        {
            throw new NotImplementedException();
        }

        public int GetWindowHandle(Session session)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetWindowHandles(Session session)
        {
            throw new NotImplementedException();
        }

        public bool SwitchToWindow(Session session, string title)
        {
            throw new NotImplementedException();
        }

        public string GetElementName(Session session, Guid elementId)
        {
            throw new NotImplementedException();
        }

        public void Clear(Session session, Guid elementId)
        {
            throw new NotImplementedException();
        }

        public Guid? FindElement(Session session, string locator, string value, Guid? elementId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> FindElements(Session session, string locator, string value, Guid? elementId)
        {
            throw new NotImplementedException();
        }

        public void SendKeys(Session session, Guid elementId, char[] keys)
        {
            throw new NotImplementedException();
        }

        public void SendKeys(Session session, string[] keys)
        {
            throw new NotImplementedException();
        }

        private IUIAutomationElement GetUIAutomationElement(Guid elementId)
        {
            var element = _elementRepository.GetById(elementId);
            var uiAutomationElement = element.GetUIAutomationElement(_automation);
            if (uiAutomationElement == null)
            {
                throw new VariableResourceNotFoundException();
            }

            return uiAutomationElement;
        }
    }
}