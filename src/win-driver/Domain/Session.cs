using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using White.Core;
using White.Core.Factory;
using White.Core.InputDevices;
using White.Core.UIItems;
using White.Core.UIItems.Finders;
using White.Core.UIItems.ListBoxItems;
using WinDriver.Exceptions;
using WinDriver.Repository;

namespace WinDriver.Domain
{
    public sealed class Session : IDisposable
    {
        private readonly IElementRepository _elementRepository;
        private readonly Capabilities _capabilities;
        private readonly Guid _sessionId;
        private readonly Application _application;

        public Session(IElementRepository elementRepository, Capabilities capabilities)
        {
            _elementRepository = elementRepository;
            _capabilities = capabilities;
            _sessionId = Guid.NewGuid();

            // TODO: inject this somehow

            if (Capabilities.App != null)
            {
                _application = Application.Launch(Capabilities.App);

                while (_application.GetWindows().Count == 0)
                {
                    Thread.Sleep(500);
                }

                _application.WaitWhileBusy();
            }
        }

        public Capabilities Capabilities
        {
            get { return _capabilities; }
        }

        public Guid SessionId
        {
            get { return _sessionId; }
        }

        public string Title
        {
            get
            {
                var window = _application.GetWindows().FirstOrDefault(x => x.IsCurrentlyActive);
                return window != null ? window.Title : _application.Process.MainWindowTitle;
            }
        }

        public IEnumerable<int> GetWindowHandles()
        {
            return _application.GetWindows().Select(x => x.AutomationElement.Current.NativeWindowHandle);
        }

        public int GetWindowHandle()
        {
            var window = _application.GetWindows().FirstOrDefault(x => x.IsCurrentlyActive);
            return window != null ? window.AutomationElement.Current.NativeWindowHandle : (int)_application.Process.MainWindowHandle;
        }

        public bool SwitchToWindow(string title)
        {
            try
            {
                var window = _application.GetWindow(title, InitializeOption.NoCache);
                return NativeMethods.SetForegroundWindow(new IntPtr(window.AutomationElement.Current.NativeWindowHandle));
            }
            catch (UIActionException)
            {
                return false;
            }
        }

        public Guid FindElement(string locator, string value, Guid? elementId)
        {
            var window = _application.GetWindow(Title);

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
                    throw new VariableResourceNotFoundException(); // TODO: should this be method not supported?
            }

            foreach (var searchCriteria in criteria)
            {
                var element = window.GetElement(searchCriteria);
                if (element != null)
                {
                    return _elementRepository.AddByHandle(element.Current.NativeWindowHandle);
                }
            }

            throw new VariableResourceNotFoundException();
        }

        public IEnumerable<Guid> FindElements(string locator, string value, Guid? elementId)
        {
            var window = _application.GetWindow(Title);
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

        public void SendKeys(Guid elementId, char[] keys)
        {
            // TODO: support modifier keys

            var window = _application.GetWindow(Title);
            var automationElement = GetAutomationElement(elementId);

            var item = new UIItem(automationElement, window);
            item.Enter(new String(keys));
        }

        public void Clear(Guid elementId)
        {
            var window = _application.GetWindow(Title);
            var automationElement = GetAutomationElement(elementId);

            var item = new UIItem(automationElement, window);
            item.Enter(String.Empty);
        }

        public void Click(Guid elementId)
        {
            var window = _application.GetWindow(Title);
            var automationElement = GetAutomationElement(elementId);

            var item = new UIItem(automationElement, window);
            item.Click();
        }

        public void DoubleClick()
        {
            Mouse.Instance.DoubleClick(Mouse.Instance.Location);
        }

        public string GetElementName(Guid elementId)
        {
            var automationElement = GetAutomationElement(elementId);

            // TODO: support more control types
            var controlType = (ControlType)automationElement.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty);
            if (controlType.Id == ControlType.ComboBox.Id)
            {
                return "select";
            }

            return controlType.LocalizedControlType;
        }

        public string GetElementAttribute(Guid elementId, string attribute)
        {
            var automationElement = GetAutomationElement(elementId);

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

        public string GetElementText(Guid elementId)
        {
            var automationElement = GetAutomationElement(elementId);

            // TODO: is this correct for controls other than ListItem?
            return automationElement.Current.Name;
        }

        public void MoveTo(Guid? elementId, int? xOffset, int? yOffset)
        {
            var point = Mouse.Instance.Location;
            double x = 0, y = 0;
            if (elementId.HasValue)
            {
                var automationElement = GetAutomationElement(elementId.Value);
                point = automationElement.Current.BoundingRectangle.TopLeft;
                x = automationElement.Current.BoundingRectangle.Width / 2;
                y = automationElement.Current.BoundingRectangle.Height / 2;
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

        public void Dispose()
        {
            if (_application != null)
            {
                _application.Dispose();
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

        private AutomationElement GetAutomationElement(Guid elementId)
        {
            var element = _elementRepository.GetById(elementId);
            var window = _application.GetWindow(Title);
            var automationElement = element.GetAutomationElement(window);
            if (automationElement == null)
            {
                throw new VariableResourceNotFoundException(); // TODO: stale element reference
            }

            return automationElement;
        }
    }
}