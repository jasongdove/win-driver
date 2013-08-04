using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using ServiceStack.Logging;
using White.Core;
using White.Core.Factory;
using White.Core.InputDevices;
using White.Core.UIItems;
using White.Core.UIItems.Finders;
using White.Core.UIItems.ListBoxItems;
using White.Core.UIItems.WindowItems;
using White.Core.WindowsAPI;
using WinDriver.Exceptions;
using WinDriver.Repository;

namespace WinDriver.Domain
{
    public sealed class Session : IDisposable
    {
        private readonly ILog _log;
        private readonly IElementRepository _elementRepository;
        private readonly Capabilities _capabilities;
        private readonly Guid _sessionId;
        private readonly Application _application;
        private readonly Timeouts _timeouts;
        private Window _window;

        public Session(ILog log, IElementRepository elementRepository, Capabilities capabilities)
        {
            _log = log;
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

            _timeouts = new Timeouts();
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
            get { return _application.Process.MainWindowTitle; }
        }

        public Timeouts Timeouts
        {
            get
            {
                return _timeouts;
            }
        }

        public IEnumerable<int> GetWindowHandles()
        {
            return _application.GetWindows().Select(x => x.AutomationElement.Current.NativeWindowHandle);
        }

        public int GetWindowHandle()
        {
            return _application.Process.MainWindowHandle.ToInt32();
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

        public Guid? FindElement(string locator, string value, Guid? elementId)
        {
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < _timeouts.Implicit)
            {
                var window = GetActiveWindow();

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

        public IEnumerable<Guid> FindElements(string locator, string value, Guid? elementId)
        {
            var window = GetActiveWindow();
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

            var window = GetActiveWindow();
            var automationElement = GetAutomationElement(elementId);

            var item = new UIItem(automationElement, window);
            item.Enter(new String(keys));
        }

        public void SendKeys(string[] keys)
        {
            var window = GetActiveWindow();
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

        public void Clear(Guid elementId)
        {
            var window = GetActiveWindow();
            var automationElement = GetAutomationElement(elementId);

            var item = new UIItem(automationElement, window);
            item.Enter(String.Empty);
        }

        public void Click(Guid elementId)
        {
            var automationElement = GetAutomationElement(elementId);
            Mouse.Instance.Click(automationElement.GetClickablePoint());
        }

        public void DoubleClick()
        {
            Mouse.Instance.DoubleClick(Mouse.Instance.Location);
            _application.WaitWhileBusy();
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
            var window = GetActiveWindow();
            var automationElement = element.GetAutomationElement(window);
            if (automationElement == null)
            {
                throw new VariableResourceNotFoundException(); // TODO: stale element reference
            }

            return automationElement;
        }

        private Window GetActiveWindow()
        {
            _application.WaitWhileBusy();

            if (_window == null || _window.AutomationElement.Current.NativeWindowHandle != _application.Process.MainWindowHandle.ToInt32())
            {
                _log.Debug("Getting window with title: " + _application.Process.MainWindowTitle);
                _window = _application.GetWindow(_application.Process.MainWindowTitle);
            }

            return _window;
        }
    }
}