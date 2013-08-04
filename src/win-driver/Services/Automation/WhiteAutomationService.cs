using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using White.Core.Factory;
using White.Core.InputDevices;
using White.Core.UIItems;
using White.Core.UIItems.Finders;
using White.Core.UIItems.ListBoxItems;
using White.Core.UIItems.WindowItems;
using White.Core.WindowsAPI;
using WinDriver.Domain;
using WinDriver.Exceptions;
using WinDriver.Repository;

namespace WinDriver.Services.Automation
{
    public class WhiteAutomationService : IAutomationService
    {
        private readonly IElementRepository _elementRepository;

        public WhiteAutomationService(IElementRepository elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public void Click(Session session, Guid elementId)
        {
            var automationElement = GetAutomationElement(session, elementId);
            Mouse.Instance.Click(automationElement.GetClickablePoint());
        }

        public void DoubleClick(Session session)
        {
            Mouse.Instance.DoubleClick(Mouse.Instance.Location);
            session.Application.WaitWhileBusy();
        }

        public void MoveTo(Session session, Guid? elementId, int? xOffset, int? yOffset)
        {
            var point = Mouse.Instance.Location;
            double x = 0, y = 0;
            if (elementId.HasValue)
            {
                var automationElement = GetAutomationElement(session, elementId.Value);
                var rect = automationElement.Current.BoundingRectangle;
                point = rect.TopLeft;
                x = rect.Width / 2;
                y = rect.Height / 2;
            }

            if (xOffset.HasValue)
            {
                x = xOffset.Value;
            }

            if (yOffset.HasValue)
            {
                y = yOffset.Value;
            }

            point.Offset(x, y);

            Mouse.Instance.Location = point;

        }

        public string GetElementText(Session session, Guid elementId)
        {
            var automationElement = GetAutomationElement(session, elementId);

            // TODO: is this correct for controls other than ListItem?
            return automationElement.Current.Name;
        }

        public string GetElementAttribute(Session session, Guid elementId, string attribute)
        {
            var automationElement = GetAutomationElement(session, elementId);

            // TODO: support more control types/attributes
            var controlType = (ControlType)automationElement.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty);
            if (controlType.Id == ControlType.ComboBox.Id)
            {
                var selectionPattern = (SelectionPattern)automationElement.GetCurrentPattern(SelectionPattern.Pattern);
                //var comboBox = new ComboBox(element, window);
                switch (attribute.ToLowerInvariant())
                {
                    case "multiple": // simulating attribute <select multiple="multiple">
                        return selectionPattern.Current.CanSelectMultiple ? "multiple" : null;
                }
            }

            return null;
        }

        public int GetWindowHandle(Session session)
        {
            return session.Application.Process.MainWindowHandle.ToInt32();
        }

        public IEnumerable<int> GetWindowHandles(Session session)
        {
            return session.Application.GetWindows().Select(x => x.AutomationElement.Current.NativeWindowHandle);
        }

        public bool SwitchToWindow(Session session, string title)
        {
            try
            {
                var window = session.Application.GetWindow(title, InitializeOption.NoCache);
                return NativeMethods.SetForegroundWindow(new IntPtr(window.AutomationElement.Current.NativeWindowHandle));
            }
            catch (UIActionException)
            {
                return false;
            }
        }

        public string GetElementName(Session session, Guid elementId)
        {
            var automationElement = GetAutomationElement(session, elementId);

            // TODO: support more control types
            var controlType = (ControlType)automationElement.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty);
            if (controlType.Id == ControlType.ComboBox.Id)
            {
                return "select";
            }

            return controlType.LocalizedControlType;
        }

        public void Clear(Session session, Guid elementId)
        {
            var window = GetActiveWindow(session);
            var automationElement = GetAutomationElement(session, elementId);

            var item = new UIItem(automationElement, window);
            item.Enter(String.Empty);
        }

        public Guid? FindElement(Session session, string locator, string value, Guid? elementId)
        {
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < session.Timeouts.Implicit)
            {
                var window = GetActiveWindow(session);

                var criteria = new List<SearchCriteria>();

                switch (locator.ToLowerInvariant())
                {
                    case "name":
                        criteria.Add(SearchCriteria.ByText(value));
                        criteria.Add(SearchCriteria.ByAutomationId(value));
                        break;
                    case "id":
                        criteria.Add(SearchCriteria.ByAutomationId(value));
                        break;
                    default:
                        throw new NotSupportedException();
                }

                foreach (var searchCriteria in criteria)
                {
                    try
                    {
                        var element = window.GetElement(searchCriteria);
                        if (element != null)
                        {
                            return _elementRepository.AddByHandle(element.Current.NativeWindowHandle);
                        }
                    }
                    catch (ElementNotAvailableException)
                    {
                        // do nothing, not available means we can't get it
                    }
                }

                Thread.Sleep(500);
            }

            return null;
        }

        public IEnumerable<Guid> FindElements(Session session, string locator, string value, Guid? elementId)
        {
            var window = GetActiveWindow(session);
            var containers = new List<IUIItemContainer> { window };

            var criteria = new List<SearchCriteria>();
            switch (locator.ToLowerInvariant())
            {
                case "name":
                    criteria.Add(SearchCriteria.ByText(value));
                    criteria.Add(SearchCriteria.ByAutomationId(value));
                    break;
                case "tag name":
                    var controlType = MapControlType(value);
                    if (controlType.Id == ControlType.ListItem.Id && elementId.HasValue)
                    {
                        var element = _elementRepository.GetById(elementId.Value);
                        var automationElement = element.GetAutomationElement(window);
                        if (automationElement.Current.ControlType.Id == ControlType.ComboBox.Id)
                        {
                            // return all list items contained within the combo box [elementId]
                            var comboBox = new ComboBox(automationElement, window);
                            return comboBox.Items.Select(x => _elementRepository.Add(new ListItemElement(element, comboBox.Items.IndexOf(x))));
                        }

                        if (automationElement.Current.ControlType.Id == ControlType.List.Id)
                        {
                            // return all list items contained within the list box [elementId]
                            var listBox = new ListBox(automationElement, window);
                            return listBox.Items.Select(x => _elementRepository.Add(new ListItemElement(element, listBox.Items.IndexOf(x))));
                        }
                    }
                    criteria.Add(SearchCriteria.ByControlType(controlType));
                    break;
                case "id":
                    criteria.Add(SearchCriteria.ByAutomationId(value));
                    break;
                default:
                    throw new VariableResourceNotFoundException(); // TODO: should this be method not supported?
            }

            var allElementIds = new List<Guid>();
            foreach (var container in containers)
            {
                foreach (var searchCriteria in criteria)
                {
                    allElementIds.AddRange(FindElements(container, searchCriteria));
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
            // TODO: support modifier keys

            var window = GetActiveWindow(session);
            var automationElement = GetAutomationElement(session, elementId);

            var item = new UIItem(automationElement, window);
            item.Enter(new String(keys));
        }

        public void SendKeys(Session session, string[] keys)
        {
            var window = GetActiveWindow(session);
            foreach (var key in keys)
            {
                // TODO: support more keys
                switch (key)
                {
                    case "\ue034":
                        window.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.F4);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        private AutomationElement GetAutomationElement(Session session, Guid elementId)
        {
            var element = _elementRepository.GetById(elementId);
            var window = GetActiveWindow(session);
            var automationElement = element.GetAutomationElement(window);
            if (automationElement == null)
            {
                throw new VariableResourceNotFoundException(); // TODO: stale element reference
            }

            return automationElement;
        }

        private Window GetActiveWindow(Session session)
        {
            session.Application.WaitWhileBusy();

            if (session.Window == null || session.Window.AutomationElement.Current.NativeWindowHandle != session.Application.Process.MainWindowHandle.ToInt32())
            {
                session.Window = session.Application.GetWindow(session.Application.Process.MainWindowTitle);
            }

            return session.Window;
        }

        private ControlType MapControlType(string tagName)
        {
            // TODO: support more control types/tag names
            switch (tagName.ToLowerInvariant())
            {
                case "option":
                    return ControlType.ListItem;
                default:
                    throw new NotSupportedException();
            }
        }

        private IEnumerable<Guid> FindElements(IUIItemContainer container, SearchCriteria criteria)
        {
            var results = container.GetMultiple(criteria);
            if (results != null && results.Any())
            {
                var handles = results.Select(x => x.AutomationElement.Current.NativeWindowHandle);
                return handles.Select(handle => _elementRepository.AddByHandle(handle)).ToList();
            }

            return new List<Guid>();
        }
    }
}