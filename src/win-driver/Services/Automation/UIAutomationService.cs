using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
            NativeMethods.DoubleClick();
            NativeMethods.WaitForInputIdle(session.Application.Process);
        }

        public void MoveTo(Session session, Guid? elementId, int? xOffset, int? yOffset)
        {
            var point = System.Windows.Forms.Cursor.Position;
            double x = 0, y = 0;
            if (elementId.HasValue)
            {
                var automationElement = GetUIAutomationElement(elementId.Value);
                var rect = automationElement.CurrentBoundingRectangle;
                point = new System.Drawing.Point(rect.left, rect.top);
                x = (rect.right - rect.left) / 2.0;
                y = (rect.bottom - rect.top) / 2.0;
            }

            if (xOffset.HasValue)
            {
                x = xOffset.Value;
            }

            if (yOffset.HasValue)
            {
                y = yOffset.Value;
            }

            point.Offset((int)x, (int)y);

            System.Windows.Forms.Cursor.Position = point;
        }

        public string GetElementText(Session session, Guid elementId)
        {
            var element = GetUIAutomationElement(elementId);
            var valuePattern = (IUIAutomationValuePattern)element.GetCurrentPattern(UIA_PatternIds.UIA_ValuePatternId);
            if (valuePattern != null)
            {
                return valuePattern.CurrentValue;
            }

            return element.CurrentName;
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
            SendKeys(session, elementId, new char[0]);
        }

        public Guid? FindElement(Session session, string locator, string value, Guid? elementId)
        {
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < session.Timeouts.Implicit)
            {
                var conditions = new List<IUIAutomationCondition>();

                switch (locator.ToLowerInvariant())
                {
                    case "name":
                        conditions.Add(_automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ValueValuePropertyId, value));
                        conditions.Add(_automation.CreatePropertyCondition(UIA_PropertyIds.UIA_AutomationIdPropertyId, value));
                        break;
                    case "id":
                        conditions.Add(_automation.CreatePropertyCondition(UIA_PropertyIds.UIA_AutomationIdPropertyId, value));
                        break;
                    default:
                        throw new NotSupportedException();
                }

                var container = elementId.HasValue
                    ? GetUIAutomationElement(elementId.Value)
                    : _automation.ElementFromHandle(session.Application.Process.MainWindowHandle);

                var condition = _automation.CreateOrConditionFromArray(conditions.ToArray());
                var element = container.FindFirst(TreeScope.TreeScope_Descendants, condition);
                if (element != null)
                {
                    return _elementRepository.AddByHandle(element.CurrentNativeWindowHandle.ToInt32());
                }

                Thread.Sleep(500);
            }

            return null;
        }

        public IEnumerable<Guid> FindElements(Session session, string locator, string value, Guid? elementId)
        {
            var container = elementId.HasValue
                ? GetUIAutomationElement(elementId.Value)
                : _automation.ElementFromHandle(session.Application.Process.MainWindowHandle);

            var conditions = new List<IUIAutomationCondition>();
            switch (locator.ToLowerInvariant())
            {
                case "name":
                    conditions.Add(_automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ValueValuePropertyId, value));
                    conditions.Add(_automation.CreatePropertyCondition(UIA_PropertyIds.UIA_AutomationIdPropertyId, value));
                    break;
                case "tag name":
                    conditions.Add(_automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, MapControlType(value)));
                    break;
                case "id":
                    conditions.Add(_automation.CreatePropertyCondition(UIA_PropertyIds.UIA_AutomationIdPropertyId, value));
                    break;
                default:
                    throw new VariableResourceNotFoundException(); // TODO: should this be method not supported?
            }

            var allElementIds = new List<Guid>();
            var condition = _automation.CreateOrConditionFromArray(conditions.ToArray());
            var results = container.FindAll(TreeScope.TreeScope_Descendants, condition);
            if (results != null)
            {
                for (int i = 0; i < results.Length; i++)
                {
                    var element = results.GetElement(i);
                    var id = elementId.HasValue && locator == "tag name" && value == "option"
                        ? _elementRepository.Add(new ListItemElement(_elementRepository.GetById(elementId.Value), i))
                        : _elementRepository.AddByHandle(element.CurrentNativeWindowHandle.ToInt32());
                    allElementIds.Add(id);
                }
            }

            if (allElementIds.Any())
            {
                return allElementIds;
            }

            throw new VariableResourceNotFoundException();
        }

        public void SendKeys(Session session, Guid elementId, char[] keys)
        {
            var keysToSend = new String(keys);

            var element = GetUIAutomationElement(elementId);
            if (element.CurrentIsEnabled != 0 && element.CurrentIsKeyboardFocusable != 0)
            {
                var valuePattern = (IUIAutomationValuePattern)element.GetCurrentPattern(UIA_PatternIds.UIA_ValuePatternId);
                if (valuePattern != null)
                {
                    valuePattern.SetValue(keysToSend);
                }
                else
                {
                    element.SetFocus();
                    System.Windows.Forms.SendKeys.SendWait("^{HOME}");
                    System.Windows.Forms.SendKeys.SendWait("^+{END}");
                    System.Windows.Forms.SendKeys.SendWait("{DEL}");
                    System.Windows.Forms.SendKeys.SendWait(keysToSend);
                }
            }
        }

        public void SendKeys(Session session, string[] keys)
        {
            var window = _automation.ElementFromHandle(session.Application.Process.MainWindowHandle);
            window.SetFocus();

            foreach (var key in keys)
            {
                // TODO: support more keys
                switch (key)
                {
                    case "\ue034":
                        System.Windows.Forms.SendKeys.SendWait("{F4}");
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
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

        private int MapControlType(string tagName)
        {
            // TODO: support more control types/tag names
            switch (tagName.ToLowerInvariant())
            {
                case "option":
                    return UIA_ControlTypeIds.UIA_ListItemControlTypeId;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}